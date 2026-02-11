namespace ReservationSportsComplex.Application.Interfaces;

public interface IWalletService
{
    Task<decimal> GetBalanceAsync(Guid userId);
    Task<bool> ChargeWalletAsync(Guid userId, decimal amount);
    Task<bool> RefundAsync(Guid userId, decimal amount);
}