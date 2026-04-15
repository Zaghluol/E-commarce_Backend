using E_commarce_Backend.Data;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class CouponService(ECommerceDbContext context) : ICouponService
    {

        // 🎟️ VALIDATE (no usage increment)
        public async Task<object> ValidateCouponAsync(string code, decimal total)
        {
            var coupon = await context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());

            if (coupon == null)
                throw new Exception("Invalid coupon");

            if (!coupon.IsActive)
                throw new Exception("Coupon is not active");

            if (coupon.ExpiryDate < DateTime.UtcNow)
                throw new Exception("Coupon expired");

            if (coupon.UsedCount >= coupon.UsageLimit)
                throw new Exception("Coupon usage limit reached");

            decimal discount = CalculateDiscount(coupon, total);

            var finalTotal = Math.Max(0, total - discount);

            return new
            {
                valid = true,
                code = coupon.Code,
                discount,
                finalTotal
            };
        }

        // 🟢 APPLY (used in checkout → increments usage)
        public async Task<decimal> ApplyCouponAsync(string code, decimal total)
        {
            var coupon = await context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());

            if (coupon == null)
                throw new Exception("Invalid coupon");

            if (!coupon.IsActive)
                throw new Exception("Coupon is not active");

            if (coupon.ExpiryDate < DateTime.UtcNow)
                throw new Exception("Coupon expired");

            if (coupon.UsedCount >= coupon.UsageLimit)
                throw new Exception("Coupon usage limit reached");

            decimal discount = CalculateDiscount(coupon, total);

            var finalTotal = Math.Max(0, total - discount);

            // 🔥 increment usage ONLY here
            coupon.UsedCount++;

            await context.SaveChangesAsync();

            return finalTotal;
        }

        // ➕ CREATE (Admin)
        public async Task CreateCouponAsync(Coupon coupon)
        {
            coupon.Code = coupon.Code.ToUpper();

            var exists = await context.Coupons
                .AnyAsync(c => c.Code == coupon.Code);

            if (exists)
                throw new Exception("Coupon already exists");

            context.Coupons.Add(coupon);
            await context.SaveChangesAsync();
        }

        // 📋 GET ALL (Admin)
        public async Task<List<Coupon>> GetAllAsync()
        {
            return await context.Coupons
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        // ✏️ UPDATE (Admin)
        public async Task UpdateAsync(int id, Coupon updatedCoupon)
        {
            var coupon = await context.Coupons.FindAsync(id);

            if (coupon == null)
                throw new Exception("Coupon not found");

            coupon.Code = updatedCoupon.Code.ToUpper();
            coupon.DiscountValue = updatedCoupon.DiscountValue;
            coupon.DiscountType = updatedCoupon.DiscountType;
            coupon.ExpiryDate = updatedCoupon.ExpiryDate;
            coupon.UsageLimit = updatedCoupon.UsageLimit;
            coupon.IsActive = updatedCoupon.IsActive;

            await context.SaveChangesAsync();
        }

        // ❌ DELETE (Admin)
        public async Task DeleteAsync(int id)
        {
            var coupon = await context.Coupons.FindAsync(id);

            if (coupon == null)
                throw new Exception("Coupon not found");

            context.Coupons.Remove(coupon);
            await context.SaveChangesAsync();
        }

        // 🧠 PRIVATE HELPER
        private decimal CalculateDiscount(Coupon coupon, decimal total)
        {
            return coupon.DiscountType switch
            {
                "percentage" => total * (coupon.DiscountValue / 100),
                "fixed" => coupon.DiscountValue,
                _ => throw new Exception("Invalid discount type")
            };
        }
    }
}