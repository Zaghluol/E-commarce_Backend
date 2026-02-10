namespace E_commarce_Backend.Models
{
    public class PendingUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string VerificationCode { get; set; }
        public DateTime CodeExpiry { get; set; }
    }

}
