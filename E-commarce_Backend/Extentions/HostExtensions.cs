using E_commarce_Backend.Data;

namespace E_commarce_Backend.Extentions
{
    public static class HostExtensions
    {
        public static async Task SeedDataAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            await SeedData.SeedRolesAndAdminAsync(scope.ServiceProvider);
        }
    }
}
