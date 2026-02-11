using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.DTOs;
using ReservationSportsComplex.Application.Interfaces;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IApplicationDbContext _context;

        public BookingController(IReservationService reservationService , IApplicationDbContext context)
        {
            _reservationService = reservationService;
            _context = context;
        }

        [HttpPost]
        [Route("Booking")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> Booking(Guid timeSlotId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var userId = Guid.Parse(userIdClaim.Value);
                var result = await  _reservationService.CreateBookingAsync(userId, timeSlotId) ;
                return Ok(new { Message = "Booking created successfully.", BookingId = result.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("My Bookings")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> GetMyBookings()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var bookings = await _context.Bookings
                .Include(b=>b.TimeSlot)
                .ThenInclude(ts=>ts.SportHall)
                .Where(b=>b.UserId==userId)
                .OrderByDescending(b=>b.BookingDate)
                .Select(b=>new UserBookingDTO
                {
                    BookingId = b.Id,
                    SportHallName =  b.TimeSlot.SportHall.Name,
                    StartTime = b.TimeSlot.StartTime,
                    EndTime = b.TimeSlot.EndTime,
                    PaidAmount = b.FinalAmount,
                    ReservedAt = b.BookingDate
                })
                .ToListAsync();
            
            return Ok(bookings);
        }


        [HttpPost]
        [Route("Cancel/{bookingId}")]
        [Authorize]

        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            
            var booking = await _context.Bookings
                .Include(b=>b.TimeSlot)
                .FirstOrDefaultAsync(b=>b.Id == bookingId && b.UserId==userId);

            if (booking == null)
            {
                return NotFound("The requested reservation was not found or does not belong to you.");
            }

            if (booking.Status == Domain.Enums.BookingStatus.Canceled)
            {
                return BadRequest("This reservation has already been canceled.");
            }

            using var transaction = await _context.BeginTransactionAsync();

            try
            {
                booking.TimeSlot.IsReserved = false;
                booking.TimeSlot.CurrentRegistrations--;
                
                var wallet = await _context.Wallets.FirstOrDefaultAsync(w=>w.UserId == userId);

                if (wallet != null)
                {
                    wallet.Balance += booking.FinalAmount;
                }

                booking.Status = Domain.Enums.BookingStatus.Canceled;
                
                await _context.SaveChangesAsync(default);
                await transaction.CommitAsync();
                
                return Ok(new { Message = "The reservation was successfully canceled and the amount was returned to your wallet.", NewBalance = wallet?.Balance  });

            }
            catch (Exception )
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while canceling the reservation.");
            }
            
            
            


        }
    }
}
