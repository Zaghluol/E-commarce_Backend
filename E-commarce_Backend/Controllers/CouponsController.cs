using E_commarce_Backend.Dtos.Coupon;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController(ICouponService couponService) : ControllerBase
    {

        // 🎟️ Validate coupon (public)
        [HttpPost("validate")]
        public async Task<IActionResult> Validate(ValidateCouponDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest("Coupon code is required");

            if (dto.CartTotal <= 0)
                return BadRequest("Cart total must be greater than 0");

            try
            {
                var result = await couponService.ValidateCouponAsync(dto.Code, dto.CartTotal);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    valid = false,
                    message = ex.Message
                });
            }
        }

        // 🔐 Create coupon (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            await couponService.CreateCouponAsync(coupon);
            return Ok(coupon);
        }

        // 📋 Get all coupons (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coupons = await couponService.GetAllAsync();
            return Ok(coupons);
        }

        // ✏️ Update coupon
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Coupon coupon)
        {
            await couponService.UpdateAsync(id, coupon);
            return Ok("Updated successfully");
        }

        // ❌ Delete coupon
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await couponService.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}