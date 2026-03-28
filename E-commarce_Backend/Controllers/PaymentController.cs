using E_commarce_Backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController(ECommerceDbContext context) : ControllerBase
    {

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] dynamic data)
        {
            bool success = data.obj.success;
            int orderId = int.Parse((string)data.obj.order.merchant_order_id);

            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.Status = success ? "Paid" : "Failed";

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
