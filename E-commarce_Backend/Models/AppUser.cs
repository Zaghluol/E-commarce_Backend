using Microsoft.AspNetCore.Identity;

namespace E_commarce_Backend.Models
{
    public class AppUser :IdentityUser
    {
        public string? FullName { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }
    }
}
