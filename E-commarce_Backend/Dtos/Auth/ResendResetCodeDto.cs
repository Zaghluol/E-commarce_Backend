using System.ComponentModel.DataAnnotations;

namespace E_commarce_Backend.Dtos.Auth
{
    public class ResendResetCodeDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
