using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.paymob;
using E_commarce_Backend.Services.Abstractions;

namespace E_commarce_Backend.Services
{
    public class PaymobService(HttpClient httpClient, IConfiguration config,ECommerceDbContext context) : IPaymobService
    {
        public async Task<string> CreatePaymentUrl(decimal amount, string orderId, string email)
        {
            // 1️⃣ AUTH
            var auth = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = config["Paymob:ApiKey"] });

            var token = (await auth.Content.ReadFromJsonAsync<AuthResponse>()).token;

            // 2️⃣ CREATE ORDER
            var orderRes = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new
                {
                    auth_token = token,
                    delivery_needed = false,
                    amount_cents = (int)(amount * 100),
                    currency = "EGP",
                    merchant_order_id = orderId
                });

            var paymobOrder = await orderRes.Content.ReadFromJsonAsync<OrderResponse>();

            // 🔥 SAVE Paymob Order ID
            var order = await context.Orders.FindAsync(int.Parse(orderId));
            order.PaymentRef = paymobOrder.id.ToString();
            await context.SaveChangesAsync();

            // 3️⃣ PAYMENT KEY
            var keyRes = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new
                {
                    auth_token = token,
                    amount_cents = (int)(amount * 100),
                    expiration = 3600,
                    order_id = paymobOrder.id,
                    currency = "EGP",
                    integration_id = int.Parse(config["Paymob:IntegrationId"]),
                    billing_data = new
                    {
                        email,
                        first_name = "NA",
                        last_name = "NA",
                        phone_number = "01000000000",
                        country = "EG",
                        city = "Cairo",
                        street = "NA",
                        building = "NA",
                        floor = "NA",
                        apartment = "NA"
                    }
                });

            var paymentKey = (await keyRes.Content.ReadFromJsonAsync<PaymentKeyResponse>()).token;

            return $"https://accept.paymob.com/api/acceptance/iframes/{config["Paymob:IframeId"]}?payment_token={paymentKey}";
        }
    }
}
