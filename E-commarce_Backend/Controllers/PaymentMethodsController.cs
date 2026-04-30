using E_commarce_Backend.Dtos.paymob;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/payment-methods")]
    [Authorize]
    public class PaymentMethodsController(IPaymentMethodService service) : ControllerBase
    {
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await service.GetUserMethodsAsync(GetUserId()));
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreatePaymentMethodDto dto)
        {
            await service.AddAsync(GetUserId(), dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(GetUserId(), id);
            return Ok();
        }

        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            await service.SetDefaultAsync(GetUserId(), id);
            return Ok();
        }
    }
}
