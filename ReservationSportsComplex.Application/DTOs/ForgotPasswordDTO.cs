namespace ReservationSportsComplex.Application.DTOs;

public class ForgotPasswordDTO
{
    public record ForgotPasswordRequest(string PhoneNumber);
    public record VerifyOtpRequest(string PhoneNumber, string Code, string NewPassword);
}