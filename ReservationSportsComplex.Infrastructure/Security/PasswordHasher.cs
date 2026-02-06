using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Infrastructure.Security
{
    public static class PasswordHasher
    {
        public static string GenerateSalt()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var combinedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
