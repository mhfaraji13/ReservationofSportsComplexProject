using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSportsComplex.Domain.Entities.Model;

namespace ReservationSportsComplex.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<bool> ExistsAsync(string phoneNumber);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
