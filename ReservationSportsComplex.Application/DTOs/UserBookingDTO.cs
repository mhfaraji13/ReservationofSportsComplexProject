namespace ReservationSportsComplex.Application.DTOs;

public class UserBookingDTO
{
    public Guid BookingId { get; set; }
    public string SportHallName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime ReservedAt { get; set; }
}