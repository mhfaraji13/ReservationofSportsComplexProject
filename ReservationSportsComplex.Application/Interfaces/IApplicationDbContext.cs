using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Domain.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace ReservationSportsComplex.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<SportHall> SportHalls { get; }
        DbSet<TimeSlot> TimeSlots { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<Wallet> Wallets { get; }
        
        Task<IDbContextTransaction>  BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync (CancellationToken cancellationToken);
    }
}
