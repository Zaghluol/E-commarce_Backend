using System.ComponentModel.DataAnnotations;

namespace E_commarce_Backend.Dtos.Address
{
    public class AddressUpdateDto
    {
        [Required]
        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }
}
