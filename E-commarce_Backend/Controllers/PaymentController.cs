using System.Security.Cryptography;
using System.Text;
using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.paymob;
using E_commarce_Backend.Models.Enums;
using E_commarce_Backend.Services;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/payment")]
public class PaymentController(ECommerceDbContext context, 
    PaymobSecurityService security,
    IConfiguration config,
    INotificationService notificationService) : ControllerBase
{
    [HttpPost("callback")]
    public async Task<IActionResult> Callback([FromBody] PaymobWebhookDto data)
    {
        // 1️⃣ Validate request
        if (data == null || data.obj == null)
            return BadRequest("Invalid payload");

        // 2️⃣ Verify HMAC (IMPORTANT SECURITY STEP)
        var isValid = VerifyHmac(data);
        if (!isValid)
            return Unauthorized("Invalid HMAC");

        var obj = data.obj;

        // 3️⃣ Get Paymob Order ID
        var paymobOrderId = obj.order?.id.ToString();

        if (string.IsNullOrEmpty(paymobOrderId))
            return BadRequest("Missing order id");

        // 4️⃣ Find your internal order
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.PaymentRef == paymobOrderId);

        if (order == null)
            return NotFound("Order not found");

        // 5️⃣ Update order status
        order.Status = obj.success ? OrderStatus.Paid : OrderStatus.Failed;

        await context.SaveChangesAsync();

        // 6️⃣ Send notification
        await notificationService.SendAsync(
            order.UserId,
            "Payment Update",
            obj.success ? "Payment successful 🎉" : "Payment failed ❌"
        );

        // 7️⃣ Clear cart ONLY if payment succeeded
        if (obj.success)
        {
            var cartItems = await context.CartItems
                .Where(c => c.UserId == order.UserId)
                .ToListAsync();

            context.CartItems.RemoveRange(cartItems);
            await context.SaveChangesAsync();
        }

        return Ok(new { Message = "Processed successfully" });
    }
    private bool VerifyHmac(PaymobWebhookDto data)
    {
        var secret = config["Paymob:HmacSecret"];

        var obj = data.obj;

        var payload =
            obj.amount_cents +
            obj.created_at +
            obj.currency +
            obj.error_occured +
            obj.has_parent_transaction +
            obj.id +
            obj.integration_id +
            obj.is_3d_secure +
            obj.is_auth +
            obj.is_capture +
            obj.is_refunded +
            obj.is_standalone_payment +
            obj.is_voided +
            obj.order?.id +
            obj.owner +
            obj.pending +
            obj.source_data?.pan +
            obj.source_data?.sub_type +
            obj.source_data?.type +
            obj.success;

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();

        return computed == data.hmac;
    }
}