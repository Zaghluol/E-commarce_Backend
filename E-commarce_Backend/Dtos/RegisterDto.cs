using System.ComponentModel.DataAnnotations;

namespace E_commarce_Backend.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression(
    @"^(010|011|012)\d{8}$",
    ErrorMessage = "Phone number must be 11 digits and start with 010, 011, or 012")]
        public string PhoneNumber { get; set; }
    }
}
