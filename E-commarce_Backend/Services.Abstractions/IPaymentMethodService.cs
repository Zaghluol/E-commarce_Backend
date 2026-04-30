using E_commarce_Backend.Dtos.paymob;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IPaymentMethodService
    {
        Task<List<PaymentMethodDto>> GetUserMethodsAsync(string userId);

        Task AddAsync(string userId, CreatePaymentMethodDto dto);

        Task DeleteAsync(string userId, int id);

        Task SetDefaultAsync(string userId, int id);
    }
}
