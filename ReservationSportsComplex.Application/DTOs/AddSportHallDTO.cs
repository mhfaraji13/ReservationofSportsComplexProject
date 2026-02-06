using ReservationSportsComplex.Domain.Entities.Model;
using ReservationSportsComplex.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Application.DTOs
{
    public class AddSportHallDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string ImageURL { get; set; } = string.Empty;
        [Required]
        public int MaxCapacity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public HallType Type { get; set; }
        
    }
}
