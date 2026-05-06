namespace E_commarce_Backend.Services.Abstractions
{
    public interface IPaymobService
    {
        Task<string> CreatePaymentUrl(decimal amount, string orderId, string email);
    }
}
