namespace E_commarce_Backend.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public string Type { get; set; } = null!;
        // e.g. "Card", "Wallet", "Fawry"

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
