using ReservationSportsComplex.Domain.Common;
using ReservationSportsComplex.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Domain.Entities.Model
{
    public class Booking 
    {
        public Guid Id { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal FinalAmount { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string? PaymentReference { get; set; }
        public Guid TimeSlotId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

    }
}
