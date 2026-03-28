namespace E_commarce_Backend.Services
{
    using System.Text;
    using System.Text.Json;
    using E_commarce_Backend.Services.Abstractions;

    public class PaymobService(HttpClient http, IConfiguration config) : IPaymobService
    {

        public async Task<string> CreatePaymentUrl(decimal amount, string orderId, string email)
        {
            // 1️⃣ Get Auth Token
            var authResponse = await http.PostAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new StringContent(JsonSerializer.Serialize(new
                {
                    api_key = config["Paymob:ApiKey"]
                }), Encoding.UTF8, "application/json"));

            var authContent = await authResponse.Content.ReadAsStringAsync();
            var authToken = JsonDocument.Parse(authContent).RootElement.GetProperty("token").GetString();

            // 2️⃣ Create Order
            var orderResponse = await http.PostAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new StringContent(JsonSerializer.Serialize(new
                {
                    auth_token = authToken,
                    delivery_needed = "false",
                    amount_cents = (int)(amount * 100),
                    currency = "EGP",
                    merchant_order_id = orderId
                }), Encoding.UTF8, "application/json"));

            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            var paymobOrderId = JsonDocument.Parse(orderContent).RootElement.GetProperty("id").GetInt32();

            // 3️⃣ Payment Key
            var paymentKeyResponse = await http.PostAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new StringContent(JsonSerializer.Serialize(new
                {
                    auth_token = authToken,
                    amount_cents = (int)(amount * 100),
                    expiration = 3600,
                    order_id = paymobOrderId,
                    billing_data = new
                    {
                        email = email,
                        first_name = "NA",
                        last_name = "NA",
                        phone_number = "01000000000",
                        country = "EG",
                        city = "Cairo",
                        street = "NA",
                        building = "NA",
                        floor = "NA",
                        apartment = "NA"
                    },
                    currency = "EGP",
                    integration_id = int.Parse(config["Paymob:IntegrationId"])
                }), Encoding.UTF8, "application/json"));

            var paymentKeyContent = await paymentKeyResponse.Content.ReadAsStringAsync();
            var paymentKey = JsonDocument.Parse(paymentKeyContent).RootElement.GetProperty("token").GetString();

            // 4️⃣ Final Payment URL
            return $"https://accept.paymob.com/api/acceptance/iframes/{config["Paymob:IframeId"]}?payment_token={paymentKey}";
        }
    }
}
