using E_commarce_Backend.Models;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface ICouponService
    {
        Task<object> ValidateCouponAsync(string code, decimal total);
        Task<decimal> ApplyCouponAsync(string code, decimal total);

        Task CreateCouponAsync(Coupon coupon);
        Task<List<Coupon>> GetAllAsync();
        Task UpdateAsync(int id, Coupon coupon);
        Task DeleteAsync(int id);
    }
}
