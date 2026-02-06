using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSportsComplex.Application.DTOs;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Entities.Model;
using ReservationSportsComplex.Infrastructure.Security;

namespace ReservationSportsComplex.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                Wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    Balance = 0
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Registration was successful and the wallet was created.");
        }
    }
}
