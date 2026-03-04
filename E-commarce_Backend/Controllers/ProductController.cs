using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService service,
        ECommerceDbContext context) : ControllerBase
    {

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await service.GetAllAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await service.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { Message = "Product not found" });

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> AdvancedSearch(
      [FromQuery] ProductSearchDto filter)
        {
            var query = context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // 🔎 Filter by product name
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(p =>
                    p.Name.Contains(filter.Name));
            }

            // 📂 Filter by category name
            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(p =>
                    p.Category.Name.Contains(filter.Category));
            }

            // 💰 Price filtering
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            // 📊 Sorting
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "price":
                        query = filter.SortDirection == "desc"
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;

                    case "name":
                        query = filter.SortDirection == "desc"
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Data = products
            });
        }
        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var createdProduct = await service.CreateAsync(dto);
            return Ok(createdProduct);
        }
        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            await service.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
    }

}