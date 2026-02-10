using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.Interfaces;

namespace ReservationSportsComplex.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public WalletController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("ChargeWallet")]

        public async Task<IActionResult> ChargeWallet(decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("The charge amount must be greater than zero.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            
            var userId = Guid.Parse(userIdClaim.Value);
            
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w=>w.UserId == userId);

            if (wallet == null)
            {
                return NotFound("The Wallet not found For this user");
            }
            
            wallet.Balance += amount;
            
            await _context.SaveChangesAsync(default);
            
            return Ok(new
            {
                Message = "Successfully charged the wallet!",
                NewBalance = wallet.Balance,
            });
        }

        [HttpGet]
        [Route("GetBalance")]

        public async Task<IActionResult> GetBalance()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w=>w.UserId == userId);
            
            return Ok(new { CurrentBalance = wallet?.Balance ?? 0 });
        }
    }
}
