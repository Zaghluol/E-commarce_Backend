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
                    CategoryId = p.CategoryId,
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
                CategoryId = product.CategoryId,

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
                CategoryId = dto.CategoryId,
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
                CategoryId = product.CategoryId,

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
        public async Task<object> AdvancedSearchAsync(ProductSearchDto filter)
        {
            var query = context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // 🔎 Name filter
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"%{filter.Name}%") ||
                    EF.Functions.Like(p.NameAr, $"%{filter.Name}%"));
            }

            // 📂 Category filter
            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.Category.Name, $"%{filter.Category}%") ||
                    EF.Functions.Like(p.Category.NameAr, $"%{filter.Category}%"));
            }

            // 💰 Price
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            // 📊 Sorting
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "price" => filter.SortDirection == "desc"
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),

                    "name" => filter.SortDirection == "desc"
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),

                    _ => query
                };
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new ProductDto // 🔥 IMPORTANT (avoid cycle)
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    CategoryId = p.Category.Id
                })
                .ToListAsync();

            return new
            {
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Data = products
            };
        }

    }
}
