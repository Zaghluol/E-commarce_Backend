using E_commarce_Backend.Dtos.paymob;
using System.Text;

namespace E_commarce_Backend.Services
{
    public class PaymobSecurityService(IConfiguration config)
    {
        public bool ValidateHmac(PaymobWebhookDto dto)
        {
            var secret = config["Paymob:HmacSecret"];

            var raw =
                $"{dto.obj.amount_cents}{dto.obj.success}{dto.obj.pending}{dto.obj.order.id}";

            using var hmac = new System.Security.Cryptography.HMACSHA512(
                Encoding.UTF8.GetBytes(secret));

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
            var generated = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return generated == dto.hmac;
        }
    }
}
