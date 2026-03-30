using E_commarce_Backend.Dtos.Address;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController(IAddressService addressService) : ControllerBase
    {
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found");
            return userId;
        }

        // GET /api/Address
        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses = await addressService.GetAddressesAsync(GetUserId());
            return Ok(addresses);
        }

        // POST /api/Address
        [HttpPost]
        public async Task<IActionResult> CreateAddress(AddressCreateDto dto)
        {
            var address = await addressService.CreateAddressAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, address);
        }

        // PUT /api/Address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, AddressUpdateDto dto)
        {
            await addressService.UpdateAddressAsync(GetUserId(), id, dto);
            return NoContent();
        }

        // DELETE /api/Address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            await addressService.DeleteAddressAsync(GetUserId(), id);
            return NoContent();
        }

        // PUT /api/Address/{id}/default
        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            await addressService.SetDefaultAddressAsync(GetUserId(), id);
            return NoContent();
        }
    }
}
