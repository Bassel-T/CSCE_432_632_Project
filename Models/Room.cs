using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSCE_432_632_Project.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public string Password { set { PasswordHash = new PasswordHasher<Room>().HashPassword(this, value); } }
        public string PasswordHash { get; set; }

        public bool ValidatePassword(string password)
        {
            var result = new PasswordHasher<Room>().VerifyHashedPassword(this, PasswordHash, password);
            return result != PasswordVerificationResult.Failed;
        }
    }
}
