namespace E_commarce_Backend.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!; // e.g. SALE10

        public decimal DiscountValue { get; set; }

        public string DiscountType { get; set; } = null!;
        // "percentage" or "fixed"

        public DateTime ExpiryDate { get; set; }

        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
