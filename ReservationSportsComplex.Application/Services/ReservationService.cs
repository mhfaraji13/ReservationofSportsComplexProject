using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSportsComplex.Application.DTOs;

namespace ReservationSportsComplex.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IApplicationDbContext applicationDbContext;

        public ReservationService(IApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }


        public async Task<bool> CheckAvailabilityAsync(Guid timeSlotId, int requestedCapacity)
        {
           var slot = await this.applicationDbContext.TimeSlots
                .Include( s => s.SportHall)
                .FirstOrDefaultAsync(s => s.Id == timeSlotId);

            if (slot == null) return false;

            return (slot.CurrentRegistrations + requestedCapacity) <= slot.SportHall.MaxCapacity;        
        }

        public async Task<Booking> CreateBookingAsync(Guid userId, Guid timeSlotId)
        {
            var user = await this.applicationDbContext.Users
                .Include(u=>u.Wallet)
                .FirstOrDefaultAsync(u=>u.Id == userId);


            var slot = await this.applicationDbContext.TimeSlots
                .Include(s => s.SportHall)
                .FirstOrDefaultAsync(s=> s.Id == timeSlotId);


            if (user == null || slot == null)
                throw new Exception("کاربر یا سانس یافت نشد !");


            if (user.Wallet.Balance < slot.SportHall.Price)
                throw new Exception("موجودی کیف پول کافی نیست !");

            user.Wallet.Balance -= slot.SportHall.Price;
            slot.CurrentRegistrations += 1;

            var booking = new Booking
            {
                Id =  Guid.NewGuid(),
                UserId = userId,
                TimeSlotId = timeSlotId,
                BookingDate = DateTime.Now,
                FinalAmount = slot.SportHall.Price,
                Status = Domain.Enums.BookingStatus.Confirmed
            };

            this.applicationDbContext.Bookings.Add(booking);
            await this.applicationDbContext.SaveChangesAsync(CancellationToken.None);

            return booking;
        }

        public async Task<List<UserBookingDTO>> GetUserBookingsAsync(Guid userId)
        {
            return await this.applicationDbContext.Bookings
                .Include(b => b.TimeSlot).ThenInclude(ts => ts.SportHall)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new UserBookingDTO {
                    BookingId = b.Id,
                    SportHallName = b.TimeSlot.SportHall.Name,
                    StartTime = b.TimeSlot.StartTime,
                    EndTime = b.TimeSlot.EndTime,
                    PaidAmount = b.FinalAmount,
                    ReservedAt = b.BookingDate
                }).ToListAsync();
        }

        public async Task<(bool Success, string Message, decimal? NewBalance)> CancelBookingAsync(Guid bookingId, Guid userId)
        {
            var booking = await this.applicationDbContext.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return (false, "Reservation not found.", null);
            if (booking.Status == Domain.Enums.BookingStatus.Canceled) return (false, "Already canceled.", null);

            using var transaction = await this.applicationDbContext.BeginTransactionAsync();
            try {
                booking.TimeSlot.IsReserved = false;
                booking.TimeSlot.CurrentRegistrations--;

                var wallet = await this.applicationDbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
                if (wallet != null) wallet.Balance += booking.FinalAmount;

                booking.Status = Domain.Enums.BookingStatus.Canceled;

                await this.applicationDbContext.SaveChangesAsync(default);
                await transaction.CommitAsync();

                return (true, "Success", wallet?.Balance);
            }
            catch {
                await transaction.RollbackAsync();
                throw; // اجازه بده خطای سرور در کنترلر مدیریت شود
            }
        }

        public async Task GenerateDailySlotsAsync(Guid hallId, DateTime date)
        {
            var hall = await this.applicationDbContext.SportHalls
                .FirstOrDefaultAsync(h => h.Id == hallId);

            if (hall == null) return;

            var startTime = new DateTime(date.Year, date.Month, date.Day, 8,0,0 );
            var endTime = new DateTime(date.Year,date.Month,date.Day, 22,0,0 );

            var currentStart = startTime;


            while(currentStart.AddMinutes(90) <= endTime)
            {
                var slotEndTime = currentStart.AddMinutes(90);

                var newSlot = new TimeSlot
                {
                    SportHallId = hallId,
                    StartTime = currentStart,
                    EndTime = slotEndTime,
                    CurrentRegistrations = 0
                };

                this.applicationDbContext.TimeSlots.Add(newSlot);

                currentStart = slotEndTime.AddMinutes(15);
            }

            await this.applicationDbContext.SaveChangesAsync(CancellationToken.None);
        }


    }
}
