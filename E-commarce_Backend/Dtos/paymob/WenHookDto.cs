namespace E_commarce_Backend.Dtos.paymob
{
    public class PaymobWebhookDto
    {
        public string hmac { get; set; }
        public PaymobObj obj { get; set; }
    }

    public class PaymobObj
    {
        public bool success { get; set; }
        public bool pending { get; set; }
        public int amount_cents { get; set; }

        public PaymobOrder order { get; set; }
    }

    public class PaymobOrder
    {
        public int id { get; set; } // Paymob Order ID
    }
}
