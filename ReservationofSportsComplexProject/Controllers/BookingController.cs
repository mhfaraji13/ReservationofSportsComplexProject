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
            try {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _reservationService.CreateBookingAsync(userId, timeSlotId);
                return Ok(new { Message = "Booking created.", BookingId = result.Id });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet]
        [Route("My Bookings")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> GetMyBookings()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _reservationService.GetUserBookingsAsync(userId);
            return Ok(result);
        }


        [HttpPost]
        [Route("Cancel/{bookingId}")]
        [Authorize]

        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var (success, message, newBalance) = await _reservationService.CancelBookingAsync(bookingId, userId);

            if (!success) return BadRequest(message);
            return Ok(new { Message = "Canceled successfully.", NewBalance = newBalance });

        }
    }
}
