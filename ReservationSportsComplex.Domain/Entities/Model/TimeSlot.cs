using ReservationSportsComplex.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Domain.Entities.Model
{
    public class TimeSlot 
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid SportHallId { get; set; }
        public SportHall SportHall { get; set; } = null!;
        public int CurrentRegistrations { get; set; }

        public int RemainingCapacity => SportHall.MaxCapacity - CurrentRegistrations;

        public bool IsAvailable => RemainingCapacity > 0;








    }
}
