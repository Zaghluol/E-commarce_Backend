namespace E_commarce_Backend.Services.Abstractions
{
    public interface IPaymobService
    {
        Task<(string paymentUrl, string paymobOrderId)> CreatePaymentUrl(
            decimal amount,
            string orderId,
            string email,
            int integrationId);
    }
}
