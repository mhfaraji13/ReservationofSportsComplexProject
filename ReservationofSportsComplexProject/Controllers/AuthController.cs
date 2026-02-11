using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.DTOs;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Entities.Model;
using ReservationSportsComplex.Domain.Enums;
using ReservationSportsComplex.Infrastructure.Security;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTTokenGenerator tokenGenerator;
        private readonly IApplicationDbContext _context;

        public AuthController(IUserRepository userRepository , IJWTTokenGenerator tokenGenerator , IApplicationDbContext context)
        {
            _userRepository = userRepository;
            this.tokenGenerator = tokenGenerator;
            _context = context;
        }

        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register(RegisterUserDTO userDto)
        {
            if (await _userRepository.ExistsAsync(userDto.PhoneNumber))
            {
                return BadRequest("This PhoneNumber is already registered.");
            }

            var salt = PasswordHasher.GenerateSalt();
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                Salt = salt,
                PasswordHash = PasswordHasher.HashPassword(userDto.Password, salt),
                Role=UserRole.Customer,
                Wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    Balance = 50000
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Registration was successful and the wallet was created.");
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userRepository.GetByPhoneNumberAsync(dto.PhoneNumber);

            if(user == null)
            {
                return Unauthorized("User Not Found.");
            }

            var incomingHash = PasswordHasher.HashPassword(dto.Password , user.Salt);

            if (user.PasswordHash!= incomingHash)
            {
                return Unauthorized("The password is incorrect.");
            }

            var token = tokenGenerator.GenerateToken(user);

            return Ok(new { Token = token , Message = "Welcome" });
        }
        
        [HttpPost("forgot-password-request")]
        public async Task<IActionResult> ForgotPasswordRequest([FromBody] ForgotPasswordDTO.ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null) return NotFound("User with this phone number not found.");

            
            var otpCode = new Random().Next(100000, 999999).ToString();
    
            user.VerificationCode = otpCode;
            user.VerificationCodeExpiry = DateTime.Now.AddMinutes(5); 

            await _context.SaveChangesAsync(default);

            
            return Ok(new { Message = "Verification code sent (Check Console/API Response for testing).", DebugCode = otpCode });
        }

        [HttpPost("verify-and-reset-password")]
        public async Task<IActionResult> VerifyAndReset([FromBody] ForgotPasswordDTO.VerifyOtpRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.PhoneNumber == request.PhoneNumber && 
                u.VerificationCode == request.Code);

            if (user == null || user.VerificationCodeExpiry < DateTime.Now)
            {
                return BadRequest("Invalid or expired verification code.");
            }
            
            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword, user.Salt);
    
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;

            await _context.SaveChangesAsync(default);

            return Ok("Password has been reset successfully. Now you can login with your new password.");
        }
    }
}
