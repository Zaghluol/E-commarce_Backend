namespace E_commarce_Backend.Dtos.paymob
{
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
