using E_commarce_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace E_commarce_Backend.Data.DataSeed
{
    public class SeedInitialData
    {
        private readonly ECommerceDbContext _dbContext;

        public SeedInitialData (ECommerceDbContext  dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task InitializeAsync()
        {
            var hasProducts = await _dbContext.Products.AnyAsync();
            var hasCategory = await _dbContext.Categories.AnyAsync();
           
            if (hasProducts && hasCategory )
                return;
            if (!hasCategory)
            {
                await SeedDataFromJsonAsync<Category>("brands.json", _dbContext.Categories);
            }
           
            await _dbContext.SaveChangesAsync();
            if (!await _dbContext.Products.AnyAsync())
            {
                await SeedDataFromJsonAsync<Product>("products.json", _dbContext.Products);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task SeedDataFromJsonAsync<T>(string FileName, DbSet<T> dbset) where T : class
        {
            var basePath = Environment.CurrentDirectory;
            var solutionPath = Path.Combine(basePath, "..");
            var FilePath = Path.GetFullPath(Path.Combine(solutionPath,  "Data", "DataSeed", "jsonfiles", FileName));
            // var FilePath =Path.GetFullPath( @"..\Presistance\Data\Initialization\DataForSeeding\" + FileName);

            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("JsonFile not found ", FilePath);
            }
            try
            {
                var DataStream = File.OpenRead(FilePath);
                var Data = await JsonSerializer.DeserializeAsync<List<T>>(DataStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (Data is not null)
                {
                    await dbset.AddRangeAsync(Data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erroe while seeding data {ex}");
            }

        }
    }
}
