using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.DTOs;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Entities.Model;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =  "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(IApplicationDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("AddSportHall")]

        public async Task<IActionResult> AddSportHall([FromBody] AddSportHallDTOAdmin dto)
        {
            var hall = _mapper.Map<SportHall>(dto);
           
            _context.SportHalls.Add(hall);
            await _context.SaveChangesAsync(default);

            var halldto = _mapper.Map<SportHallDTO>(hall);
            
            return Ok(new { Message = "New hall successfully registered." , HallId = halldto.Id });
        }


        [HttpPost]
        [Route("GenerateSlots")]
        public async Task<IActionResult> GenerateSlots(Guid sportHallId, DateTime date, int durationMinutes)
        {
            var hall = await  _context.SportHalls.FindAsync(sportHallId);

            if (hall == null)
            {
                return NotFound("Sport hall not found");
            }

            var slots = new List<TimeSlot>();
            var startTime = date.Date.AddHours(8);
            var endTime = date.Date.AddHours(22);

            while (startTime.AddMinutes(durationMinutes) <= endTime)
            {
                slots.Add(new TimeSlot
                {
                    Id =  Guid.NewGuid(),
                    SportHallId =  sportHallId,
                    StartTime = startTime,
                    EndTime = startTime.AddMinutes(durationMinutes),
                    IsReserved =  false,
                    Price = hall.Price,
                });
                
                startTime = startTime.AddMinutes(durationMinutes);
            }
            
            _context.TimeSlots.AddRange(slots);
            await _context.SaveChangesAsync(default);
            
            return Ok($"{slots.Count} New Slots For Date {date.ToShortDateString()} Was Created.");
        }

        [HttpGet]
        [Route("AllBookings")]

        public async Task<IActionResult> GetAllBookings()
        {
            var allBookings = await _context.Bookings
                .Include(b=>b.User)
                .Include(b=>b.TimeSlot)
                .ThenInclude(ts=>ts.SportHall)
                .OrderByDescending(b=>b.BookingDate)
                .Select(b=> new
                {
                    b.Id,
                    CustomerName = $"{b.User.FirstName} {b.User.LastName}",
                    b.User.PhoneNumber,
                    SportHallName = b.TimeSlot.SportHall.Name,
                    b.TimeSlot.StartTime,
                    b.TimeSlot.EndTime,
                    b.FinalAmount,
                    b.Status
                })
                .ToListAsync();
            
            return Ok(allBookings);
        }
        
    }
}
