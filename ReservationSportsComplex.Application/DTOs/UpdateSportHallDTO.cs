using ReservationSportsComplex.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Application.DTOs
{
    public class UpdateSportHallDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
        public HallType Type { get; set; }
    }
}
