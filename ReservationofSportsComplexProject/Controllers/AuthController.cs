using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IUserRepository userRepository , IJWTTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            this.tokenGenerator = tokenGenerator;
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
    }
}
