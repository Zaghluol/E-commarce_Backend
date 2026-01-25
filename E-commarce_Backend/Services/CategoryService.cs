using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class CategoryService(ECommerceDbContext context) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task UpdateAsync(int id, CreateCategoryDto dto)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
                throw new Exception("Category not found.");

            category.Name = dto.Name;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
                return;

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }


}
