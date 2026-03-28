using E_commarce_Backend.Dtos.Orders;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersControllerOrdersController(IOrderService orderService) : ControllerBase
{

    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID claim is missing");

        return userId;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        var result = await orderService.CheckoutAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyOrders()
    {
        var result = await orderService.GetMyOrdersAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var result = await orderService.GetOrderDetailsAsync(GetUserId(), id);
        return Ok(result);
    }
}