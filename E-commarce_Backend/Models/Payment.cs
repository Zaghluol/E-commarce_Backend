namespace E_commarce_Backend.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Provider { get; set; } // Paymob
        public string PaymentId { get; set; }
        public string Status { get; set; }
    }
}
