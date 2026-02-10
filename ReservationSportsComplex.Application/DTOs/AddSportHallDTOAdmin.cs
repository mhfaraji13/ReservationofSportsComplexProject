using System.ComponentModel.DataAnnotations;
using ReservationSportsComplex.Domain.Enums;

namespace ReservationSportsComplex.Application.DTOs;

public class AddSportHallDTOAdmin
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