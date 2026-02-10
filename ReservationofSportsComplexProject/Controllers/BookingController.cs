using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSportsComplex.Application.Interfaces;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public BookingController(IReservationService reservationService)
        {
            _reservationService = reservationService;
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
    }
}
