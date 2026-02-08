using System.ComponentModel.DataAnnotations;

namespace E_commarce_Backend.Dtos
{
    public class ResendVerificationCodeDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

}
