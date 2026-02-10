using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Domain.Entities.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Infrastructure.Security
{
    public class JWTTokenGenerator : IJWTTokenGenerator
    {
        private readonly IConfiguration config;

        public JWTTokenGenerator(IConfiguration config)
        {
            this.config = config;
        }
        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Secret"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["JwtSettings:Issuer"],
                audience: config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(config["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
