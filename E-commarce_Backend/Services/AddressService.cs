using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Address;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class AddressService(ECommerceDbContext context) : IAddressService
    {
        public async Task<IEnumerable<AddressResponseDto>> GetAddressesAsync(string userId)
        {
            var addresses = await context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.Id)
                .Select(a => new AddressResponseDto
                {
                    Id = a.Id,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    City = a.City,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();

            return addresses;
        }

        public async Task<AddressResponseDto> CreateAddressAsync(string userId, AddressCreateDto dto)
        {
            // If this address is set as default, unset any existing default for this user
            if (dto.IsDefault)
            {
                await UnsetDefaultAddressAsync(userId);
            }

            var address = new Address
            {
                UserId = userId,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                IsDefault = dto.IsDefault
            };

            context.Addresses.Add(address);
            await context.SaveChangesAsync();

            return new AddressResponseDto
            {
                Id = address.Id,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };
        }

        public async Task UpdateAddressAsync(string userId, int addressId, AddressUpdateDto dto)
        {
            var address = await context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
                throw new Exception("Address not found");

            // If updating to default and it wasn't already default, unset others
            if (dto.IsDefault && !address.IsDefault)
            {
                await UnsetDefaultAddressAsync(userId);
            }

            address.AddressLine1 = dto.AddressLine1;
            address.AddressLine2 = dto.AddressLine2;
            address.City = dto.City;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;
            address.IsDefault = dto.IsDefault;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(string userId, int addressId)
        {
            var address = await context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
                throw new Exception("Address not found");

            context.Addresses.Remove(address);
            await context.SaveChangesAsync();
        }

        public async Task SetDefaultAddressAsync(string userId, int addressId)
        {
            var address = await context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
                throw new Exception("Address not found");

            await UnsetDefaultAddressAsync(userId);

            address.IsDefault = true;
            await context.SaveChangesAsync();
        }

        // Helper method to unset any existing default address for a user
        private async Task UnsetDefaultAddressAsync(string userId)
        {
            var existingDefault = await context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

            if (existingDefault != null)
            {
                existingDefault.IsDefault = false;
                await context.SaveChangesAsync();
            }
        }
    }
}
