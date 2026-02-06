using ReservationSportsComplex.Domain.Entities.Model;
using ReservationSportsComplex.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Application.DTOs
{
    public class SportHallDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
        public HallType Type { get; set; }
        public ICollection<TimeSlotDTO> TimeSlots { get; set; } = new List<TimeSlotDTO>();
    }
}
