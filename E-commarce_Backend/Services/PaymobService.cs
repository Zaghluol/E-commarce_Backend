using E_commarce_Backend.Dtos.paymob;
using E_commarce_Backend.Services.Abstractions;

namespace E_commarce_Backend.Services
{
    public class PaymobService(HttpClient httpClient, IConfiguration config) : IPaymobService
    {
        public async Task<(string paymentUrl, string paymobOrderId)> CreatePaymentUrl(
            decimal amount,
            string orderId,
            string email,
            int integrationId)
        {
            // 1️⃣ AUTH
            var authResponse = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = config["Paymob:ApiKey"] });

            var authResult = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();
            var token = authResult.token;

            // 2️⃣ CREATE ORDER
            var orderResponse = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new
                {
                    auth_token = token,
                    delivery_needed = false,
                    amount_cents = (int)(amount * 100),
                    currency = "EGP",
                    merchant_order_id = orderId,
                    items = new object[] { }
                });

            var orderResult = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>();

            // 3️⃣ PAYMENT KEY
            var paymentKeyResponse = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new
                {
                    auth_token = token,
                    amount_cents = (int)(amount * 100),
                    expiration = 3600,
                    order_id = orderResult.id,
                    billing_data = new
                    {
                        email = email,
                        first_name = "Test",
                        last_name = "User",
                        phone_number = "01000000000",
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        city = "Cairo",
                        country = "EG",
                        state = "NA"
                    },
                    currency = "EGP",
                    integration_id = integrationId
                });

            var paymentKeyResult = await paymentKeyResponse.Content.ReadFromJsonAsync<PaymentKeyResponse>();

            // 4️⃣ URL
            var url = $"https://accept.paymob.com/api/acceptance/iframes/{config["Paymob:IframeId"]}?payment_token={paymentKeyResult.token}";

            // 🔴 RETURN BOTH
            return (url, orderResult.id.ToString());
        }
    }
}
