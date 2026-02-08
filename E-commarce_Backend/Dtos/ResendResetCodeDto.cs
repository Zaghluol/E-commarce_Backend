using System.ComponentModel.DataAnnotations;

namespace E_commarce_Backend.Dtos
{
    public class ResendResetCodeDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
