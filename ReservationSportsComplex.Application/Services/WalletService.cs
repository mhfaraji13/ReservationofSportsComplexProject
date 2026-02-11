using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.Interfaces;

namespace ReservationSportsComplex.Application.Services;

public class WalletService :  IWalletService
{
    private readonly IApplicationDbContext _context;

    public WalletService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetBalanceAsync(Guid userId)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w=>w.UserId == userId);
        return wallet?.Balance ?? 0;
    }

    public async Task<bool> ChargeWalletAsync(Guid userId, decimal amount)
    {

        if (amount <= 0)
        {
            return false;
        }
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            return false;
        }
        wallet.Balance += amount;
        await _context.SaveChangesAsync(default);
        return true;
    }

    public async Task<bool> RefundAsync(Guid userId, decimal amount)
    {
        return await ChargeWalletAsync(userId, amount);
    }
}