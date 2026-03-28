namespace E_commarce_Backend.Dtos.Orders
{
    public class CheckoutDto
    {
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }

        // optional (for future payment)
        public string PaymentMethod { get; set; } // "cash" / "card"
    }
}
