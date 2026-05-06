using E_commarce_Backend.Dtos.Admin;
using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Models.Enums;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardAsync();

        Task<List<AdminUserDto>> GetUsersAsync();

        Task<List<AdminOrderDto>> GetOrdersAsync();

        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);

        Task UpdateOrderStatusAsync(int orderId, string status); // optional helper

        Task<List<ConversationDto>> GetSupportConversationsAsync();
    }
}