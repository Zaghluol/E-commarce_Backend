using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.paymob;
using E_commarce_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/payment")]
public class PaymentController(ECommerceDbContext context, PaymobSecurityService security) : ControllerBase
{
    [HttpPost("callback")]
    public async Task<IActionResult> Callback([FromBody] PaymobWebhookDto dto)
    {
        // 🔴 1. Validate HMAC
        if (!security.ValidateHmac(dto))
            return BadRequest("Invalid HMAC");

        // 🔴 2. Check success AND not pending
        if (!dto.obj.success || dto.obj.pending)
            return Ok(); // ignore

        // 🔴 3. Find order using Paymob Order ID
        var order = await context.Orders
            .FirstOrDefaultAsync(x => x.PaymentRef == dto.obj.order.id.ToString());

        if (order == null)
            return NotFound();

        // 🔴 4. Update order
        order.Status = "Paid";

        await context.SaveChangesAsync();

        return Ok();
    }
}