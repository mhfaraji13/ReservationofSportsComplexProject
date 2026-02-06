using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext , IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<SportHall> SportHalls => Set<SportHall>();

        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<Wallet> Wallets => Set<Wallet>();


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<Booking>()
         .Property(b => b.FinalAmount)
         .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SportHall>()
         .Property(s => s.Price)
         .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18,2)");
        }
    }
}
