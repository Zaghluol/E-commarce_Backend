namespace E_commarce_Backend.Services
{
    //using System.Text;
    //using System.Text.Json;
    //using E_commarce_Backend.Data;
    //using E_commarce_Backend.Services.Abstractions;

    //public class PaymobService(HttpClient http, IConfiguration config,ECommerceDbContext context) : IPaymobService
    //{

    //    public async Task<string> CreatePaymentUrl(decimal amount, string orderId, string email)
    //    {
    //        // 1️⃣ AUTH
    //        var authResponse = await http.PostAsync(
    //            "https://accept.paymob.com/api/auth/tokens",
    //            new StringContent(JsonSerializer.Serialize(new
    //            {
    //                api_key = config["Paymob:ApiKey"]
    //            }), Encoding.UTF8, "application/json"));

    //        var authContent = await authResponse.Content.ReadAsStringAsync();
    //        var authToken = JsonDocument.Parse(authContent)
    //            .RootElement.GetProperty("token")
    //            .GetString();

    //        // 2️⃣ CREATE ORDER
    //        var orderResponse = await http.PostAsync(
    //            "https://accept.paymob.com/api/ecommerce/orders",
    //            new StringContent(JsonSerializer.Serialize(new
    //            {
    //                auth_token = authToken,
    //                delivery_needed = false,
    //                amount_cents = (int)(amount * 100),
    //                currency = "EGP",

    //                // 🔴 IMPORTANT: keep your internal order id
    //                merchant_order_id = orderId
    //            }), Encoding.UTF8, "application/json"));

    //        var orderContent = await orderResponse.Content.ReadAsStringAsync();
    //        var paymobOrderId = JsonDocument.Parse(orderContent)
    //            .RootElement.GetProperty("id")
    //            .GetInt32();

    //        // 🔴 IMPORTANT FIX: SAVE PAYMOB ORDER ID
    //        var order = await context.Orders.FindAsync(int.Parse(orderId));
    //        if (order != null)
    //        {
    //            order.PaymentRef = paymobOrderId.ToString(); // 🔥 REQUIRED FOR WEBHOOK MATCHING
    //            order.Status = "PendingPayment";
    //            await context.SaveChangesAsync();
    //        }

    //        // 3️⃣ PAYMENT KEY
    //        var paymentKeyResponse = await http.PostAsync(
    //            "https://accept.paymob.com/api/acceptance/payment_keys",
    //            new StringContent(JsonSerializer.Serialize(new
    //            {
    //                auth_token = authToken,
    //                amount_cents = (int)(amount * 100),
    //                expiration = 3600,
    //                order_id = paymobOrderId,

    //                billing_data = new
    //                {
    //                    email = email,
    //                    first_name = "NA",
    //                    last_name = "NA",
    //                    phone_number = "01000000000",
    //                    country = "EG",
    //                    city = "Cairo",
    //                    street = "NA",
    //                    building = "NA",
    //                    floor = "NA",
    //                    apartment = "NA"
    //                },

    //                currency = "EGP",
    //                integration_id = int.Parse(config["Paymob:IntegrationId"])
    //            }), Encoding.UTF8, "application/json"));

    //        var paymentKeyContent = await paymentKeyResponse.Content.ReadAsStringAsync();

    //        var paymentKey = JsonDocument.Parse(paymentKeyContent)
    //            .RootElement.GetProperty("token")
    //            .GetString();

    //        // 4️⃣ FINAL URL
    //        return $"https://accept.paymob.com/api/acceptance/iframes/{config["Paymob:IframeId"]}?payment_token={paymentKey}";
    //    }
    //}
using System.Net.Http;
using System.Net.Http.Json;
    using E_commarce_Backend.Dtos.paymob;
    using E_commarce_Backend.Services.Abstractions;
    using Microsoft.Extensions.Configuration;

public class PaymobService(HttpClient httpClient, IConfiguration config) : IPaymobService
    {
        public async Task<string> CreatePaymentUrl(decimal amount, string orderId, string email)
        {
            // 1. Authenticate
            var authResponse = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = config["Paymob:ApiKey"] });
            var authResult = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();
            var token = authResult.token;

            // 2. Register Order
            var orderResponse = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new
                {
                    auth_token = token,
                    delivery_needed = "false",
                    amount_cents = (int)(amount * 100),
                    currency = "EGP",
                    items = new object[] { }
                });
            var orderResult = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>();

            // 3. Get Payment Key
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
                        // Add other required billing fields here
                        first_name = "Test",
                        last_name = "User",
                        phone_number = "01000000000",
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        city = "NA",
                        country = "NA",
                        state = "NA"
                    },
                    currency = "EGP",
                    integration_id = int.Parse(config["Paymob:IntegrationId"])
                });
            var paymentKeyResult = await paymentKeyResponse.Content.ReadFromJsonAsync<PaymentKeyResponse>();

            // 4. Build Payment URL
            var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{config["Paymob:IframeId"]}?payment_token={paymentKeyResult.token}";
            return paymentUrl;
        }
    }
}
