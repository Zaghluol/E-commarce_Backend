namespace E_commarce_Backend.Dtos.Orders
{
    public class CheckoutDto
    {
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }
        public string PaymentMethod { get; set; }

        public string? CouponCode { get; set; } // 🔥 NEW
    }
}
