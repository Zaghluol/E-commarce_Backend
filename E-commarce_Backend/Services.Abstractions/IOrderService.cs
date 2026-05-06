using E_commarce_Backend.Dtos.Orders;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IOrderService
    {
        Task<object> CheckoutAsync(string userId, CheckoutDto dto);
        Task<object> GetMyOrdersAsync(string userId);
        Task<object> GetOrderDetailsAsync(string userId, int orderId);
    }
}
