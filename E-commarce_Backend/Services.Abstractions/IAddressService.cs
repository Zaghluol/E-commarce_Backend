using E_commarce_Backend.Dtos.Address;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressResponseDto>> GetAddressesAsync(string userId);
        Task<AddressResponseDto> CreateAddressAsync(string userId, AddressCreateDto dto);
        Task UpdateAddressAsync(string userId, int addressId, AddressUpdateDto dto);
        Task DeleteAddressAsync(string userId, int addressId);
        Task SetDefaultAddressAsync(string userId, int addressId);
    }
}
