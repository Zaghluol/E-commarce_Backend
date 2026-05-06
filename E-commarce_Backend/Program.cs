
using E_commarce_Backend.Data;
using E_commarce_Backend.Data.DataSeed;
using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Extentions;
using E_commarce_Backend.Profiles;
using E_commarce_Backend.Services;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace E_commarce_Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDatabaseAndIdentity(builder.Configuration);
            builder.Services.AddEcommarceDbContexts(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddControllers();
            builder.Services.AddSignalR();


            // Add services to the container.


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            await app.MigrateDataBase();
            await app.SeedingData();

            app.MapHub<SupportHub>("/supportHub");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
                app.UseSwagger();
                app.UseSwaggerUI();

            await app.SeedDataAsync();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.SeedRolesAndAdminAsync(services);
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
