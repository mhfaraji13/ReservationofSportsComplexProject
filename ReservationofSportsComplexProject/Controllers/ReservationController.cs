using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Interfaces;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ISportHall _sportHall;
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;

        public ReservationController(ISportHall sportHall , IMapper mapper , IReservationService reservationService)
        {
            _sportHall = sportHall;
            _mapper = mapper;
            _reservationService = reservationService;
        }

        [HttpPost("{id}/generate-daily-slots")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GenerateSlots(Guid id, DateTime date,
            [FromServices] IReservationService resrReservationService)
        {
            await resrReservationService.GenerateDailySlotsAsync(id, date);

            return Ok(new { message = $"Sessions of the day {date.ToShortDateString()} Successfully created." });
        }
    }
}
