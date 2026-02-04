using ReservationSportsComplex.Domain.Common;
using ReservationSportsComplex.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Domain.Entities.Model
{
    public class SportHall : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public HallType Type { get; set; }
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }

        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    }
}
