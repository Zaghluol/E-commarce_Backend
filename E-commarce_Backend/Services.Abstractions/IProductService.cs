using E_commarce_Backend.Dtos.Product;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(int id, CreateProductDto dto);
        Task DeleteAsync(int id);
    }

}
