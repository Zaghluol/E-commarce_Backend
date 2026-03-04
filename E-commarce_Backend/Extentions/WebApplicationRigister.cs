using E_commarce_Backend.Data;
using E_commarce_Backend.Data.DataSeed;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Extentions
{
    public static class WebApplicationRigister
    {
        public static async Task<WebApplication> MigrateDataBase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
            var hasPendingMigrations = await dbcontext.Database.GetPendingMigrationsAsync();
            if (hasPendingMigrations.Any())
            {
                await dbcontext.Database.MigrateAsync();
            }
            return app;

        }
        public static async Task<WebApplication> SeedingData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dataInitilizer = scope.ServiceProvider.GetRequiredService<SeedInitialData>();
            await dataInitilizer.InitializeAsync();
            return app;
        }
    }
}
