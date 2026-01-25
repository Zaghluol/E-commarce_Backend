using E_commarce_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace E_commarce_Backend.Data
{
        public static class SeedData
        {
            public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                // Roles to create
                string[] roles = { "Admin", "Customer" };

                foreach (string role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Create Admin User
                string adminEmail = "admin@shop.com";

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Super Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
        }
}
