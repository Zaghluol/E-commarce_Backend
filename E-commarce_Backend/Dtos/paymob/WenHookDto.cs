namespace E_commarce_Backend.Dtos.paymob
{
    public class PaymobWebhookDto
    {
        public string hmac { get; set; }
        public PaymobObj obj { get; set; }
    }

    public class PaymobObj
    {
        public int id { get; set; }
        public bool success { get; set; }

        public int amount_cents { get; set; }
        public string currency { get; set; }

        public bool is_auth { get; set; }
        public bool is_3d_secure { get; set; }
        public bool is_capture { get; set; }
        public bool is_standalone_payment { get; set; }
        public bool is_voided { get; set; }
        public bool is_refunded { get; set; }

        public bool error_occured { get; set; }
        public bool has_parent_transaction { get; set; }
        public bool pending { get; set; }

        public int integration_id { get; set; }

        public string created_at { get; set; }
        public int owner { get; set; }

        public PaymobOrder order { get; set; }

        // 🔥 THIS WAS MISSING
        public SourceData source_data { get; set; }
    }
    public class PaymobOrder
    {
        public int id { get; set; }
    }
    public class SourceData
    {
        public string pan { get; set; }        // masked card number
        public string type { get; set; }       // card / wallet
        public string sub_type { get; set; }   // visa / master / vodafone
    }
}
