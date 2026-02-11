using ReservationSportsComplex.Domain.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSportsComplex.Application.DTOs;

namespace ReservationSportsComplex.Application.Interfaces
{
    public interface IReservationService
    {
        Task GenerateDailySlotsAsync(Guid hallId , DateTime date);
        Task<bool> CheckAvailabilityAsync(Guid timeSlotId , int requestedCapacity);
        Task<Booking> CreateBookingAsync(Guid userId, Guid timeSlotId);
        
        Task<List<UserBookingDTO>>  GetUserBookingsAsync(Guid userId);
        
        Task<(bool Success , string Message , decimal? NewBalance)> CancelBookingAsync(Guid bookingId , Guid userId);
    }
}
