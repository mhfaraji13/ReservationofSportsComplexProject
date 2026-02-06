using ReservationSportsComplex.Domain.Common;
using ReservationSportsComplex.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Domain.Entities.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public Wallet Wallet { get; set; } = null!;



        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;


        
        public DateTime? LastLoginDate { get; set; }



        public bool IsBlocked { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.Customer;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
