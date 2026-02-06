using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Domain.Entities.Model;
using ReservationSportsComplex.Domain.Interfaces;
using ReservationSportsComplex.Infrastructure.Data;

namespace ReservationSportsComplex.Infrastructure.Repositories
{
    public class SportHallRepository : ISportHall
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SportHallRepository(ApplicationDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<SportHall> CreateAsync(SportHall hall)
        {
            await _context.SportHalls.AddAsync(hall);
            await _context.SaveChangesAsync();
            return hall;
        }

        public async Task<SportHall?> DeleteAsync(Guid id)
        {
            var sporthall = await _context.SportHalls.FirstOrDefaultAsync(x => x.Id == id);

            if (sporthall == null)
            {
                return null;
            }

            _context.SportHalls.Remove(sporthall);

            await _context.SaveChangesAsync();

            return sporthall;
        }

        public async Task<List<SportHall>> GetAllAsync()
        {
            return await _context.SportHalls.ToListAsync();
        }

        public async Task<SportHall?> GetByIdAsync(Guid id)
        {
            return await _context.SportHalls.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SportHall?> UpdateAsync(Guid id, SportHall hall)
        {
            var existingSportHall = await _context.SportHalls.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (existingSportHall == null)
            {
                return null;
            }

            var updateEntity = _mapper.Map<SportHall>(hall);

            updateEntity.Id = id;

            _context.Entry(updateEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return updateEntity;
        }
    }
}
