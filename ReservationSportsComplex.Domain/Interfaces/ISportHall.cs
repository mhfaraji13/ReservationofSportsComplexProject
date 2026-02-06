using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSportsComplex.Domain.Entities.Model;

namespace ReservationSportsComplex.Domain.Interfaces
{
    public interface ISportHall
    {
        Task<SportHall> CreateAsync(SportHall hall);

        Task<List<SportHall>> GetAllAsync();

        Task<SportHall?> GetByIdAsync(Guid id);

        Task<SportHall?> UpdateAsync(Guid id , SportHall hall);

        Task<SportHall?> DeleteAsync(Guid id);
    }
}
