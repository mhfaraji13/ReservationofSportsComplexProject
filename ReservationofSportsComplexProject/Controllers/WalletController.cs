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
        private readonly IWalletService _walletService;

        public WalletController(IApplicationDbContext context , IWalletService  walletService)
        {
            _context = context;
            _walletService = walletService;
        }

        [HttpPost]
        [Route("ChargeWallet")]

        public async Task<IActionResult> ChargeWallet(decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        
            var result = await _walletService.ChargeWalletAsync(userId, amount);
        
            if (!result) return NotFound("Wallet not found.");
        
            return Ok("Wallet successfully recharged.");
        }

        [HttpGet]
        [Route("GetBalance")]

        public async Task<IActionResult> GetBalance()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var balance = await _walletService.GetBalanceAsync(userId);
            return Ok(new { CurrentBalance = balance });
        }
    }
}
