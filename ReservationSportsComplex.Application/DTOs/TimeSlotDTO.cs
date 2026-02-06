using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Application.DTOs
{
    public class TimeSlotDTO
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsReserved { get; set; }
        public Guid SportHallId { get; set; }
        public int CurrentRegistrations { get; set; }
        public int RemainingCapacity { get; set; } 
        public bool IsAvailable { get; set; }
    }
}
