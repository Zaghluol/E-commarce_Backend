namespace E_commarce_Backend.Dtos.Profile
{
    public class UserProfileDto
    {
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
