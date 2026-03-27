using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Product;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{

    public class ProductService(ECommerceDbContext context) : IProductService
    {

        // 🔹 GET ALL PRODUCTS
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    NameAr = p.NameAr,
                    Price = p.Price,
                    Description = p.Description,
                    DescriptionAr = p.DescriptionAr,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId ,
                })
                .ToListAsync();
        }

        // 🔹 GET PRODUCT BY ID
        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                NameAr = product.NameAr,
                Price = product.Price,
                Description = product.Description,
                DescriptionAr = product.DescriptionAr,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId ,
               
            };
        }

        // 🔹 CREATE PRODUCT
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // Validate Category
            var category = await context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                throw new Exception("Category not found.");

            var product = new Product
            {
                Name = dto.Name,
                NameAr = dto.NameAr,
                Price = dto.Price,
                Description = dto.Description,
                DescriptionAr = dto.DescriptionAr,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId ,
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                NameAr = product.NameAr,
                Price = product.Price,
                Description = product.Description,
                DescriptionAr = product.DescriptionAr,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId ,
                
            };
        }

        // 🔹 UPDATE PRODUCT
        public async Task UpdateAsync(int id, CreateProductDto dto)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                throw new Exception("Product not found.");

            product.Name = dto.Name;
            product.NameAr = dto.NameAr;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.DescriptionAr = dto.DescriptionAr;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await context.SaveChangesAsync();
        }

        // 🔹 DELETE PRODUCT
        public async Task DeleteAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return;

            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }

}
