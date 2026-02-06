using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.Interfaces;
using System.Threading.Tasks;
using AutoMapper;
using ReservationSportsComplex.Application.DTOs;
using ReservationSportsComplex.Domain.Entities.Model;
using ReservationSportsComplex.Domain.Interfaces;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportHallController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ISportHall _sportHall;

        public SportHallController(IMapper mapper , IApplicationDbContext context , ISportHall sportHall)
        {
            _mapper = mapper;
            _context = context;
            _sportHall = sportHall;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddSportHallDTO addSportHallDto)
        {
            var sporthallmodel = _mapper.Map<SportHall>(addSportHallDto);

            sporthallmodel = await _sportHall.CreateAsync(sporthallmodel);

            var sporthallDTO = _mapper.Map<SportHallDTO>(sporthallmodel);

            var locationURI = $"/api/SportHall/{sporthallDTO.Id}";
            return Created(locationURI, sporthallDTO);


        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var sporthallsmodel = await _sportHall.GetAllAsync();

            var sporthallDTO = _mapper.Map<IEnumerable<SportHallDTO>>(sporthallsmodel);

            return Ok(sporthallDTO);
        }

        [HttpGet]
        [Route("{id:Guid}")]

        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var hall = await _sportHall.GetByIdAsync(id);

            if (hall == null)
            {
                return NotFound();
            }

            var hallDTO = _mapper.Map<SportHallDTO>(hall);

            return Ok(hallDTO);
        }


        [HttpPut]
        [Route("{id:Guid}")]

        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSportHallDTO updateSportHallDto)
        {
            var hallmodel = _mapper.Map<SportHall>(updateSportHallDto);
            
            var updatehall = await _sportHall.UpdateAsync(id, hallmodel);
            
            if (updatehall == null)
            {
                return NotFound();
            }
            
            var hallDTO = _mapper.Map<SportHallDTO>(updatehall);
            
            return Ok(hallDTO);
        }

        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletehall = await _sportHall.DeleteAsync(id);
            if (deletehall == null)
            {
                return NotFound();
            }
            var hallDTO = _mapper.Map<SportHallDTO>(deletehall);
            return Ok(hallDTO);
        }
    }
}
