using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.paymob;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly ECommerceDbContext context;

        public PaymentMethodService(ECommerceDbContext context)
        {
            this.context = context;
        }

        public async Task<List<PaymentMethodDto>> GetUserMethodsAsync(string userId)
        {
            return await context.PaymentMethods
                .Where(x => x.UserId == userId)
                .Select(x => new PaymentMethodDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    IsDefault = x.IsDefault
                })
                .ToListAsync();
        }

        public async Task AddAsync(string userId, CreatePaymentMethodDto dto)
        {
            var method = new PaymentMethod
            {
                UserId = userId,
                Type = dto.Type
            };

            context.PaymentMethods.Add(method);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId, int id)
        {
            var method = await context.PaymentMethods
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (method == null)
                throw new Exception("Payment method not found");

            context.PaymentMethods.Remove(method);
            await context.SaveChangesAsync();
        }

        public async Task SetDefaultAsync(string userId, int id)
        {
            var methods = await context.PaymentMethods
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var m in methods)
                m.IsDefault = m.Id == id;

            await context.SaveChangesAsync();
        }
    }
}
