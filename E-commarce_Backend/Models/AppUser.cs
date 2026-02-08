using Microsoft.AspNetCore.Identity;

namespace E_commarce_Backend.Models
{
    public class AppUser :IdentityUser
    {
        public string? FullName { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailCodeExpiry { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }
        public DateTime? LastVerificationCodeSentAt { get; set; }
        public DateTime? LastResetCodeSentAt { get; set; }

    }
}
