using System.Text;
using E_commarce_Backend.Data;
using E_commarce_Backend.Services.Abstractions;
using E_commarce_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using E_commarce_Backend.Repository;
using E_commarce_Backend.Data.DataSeed;
using Microsoft.OpenApi.Models;
using E_commarce_Backend.Profiles;
using E_commarce_Backend.Models.User;

namespace E_commarce_Backend.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseAndIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            // Identity
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
       
        public static IServiceCollection AddEcommarceDbContexts(this IServiceCollection services, IConfiguration config)
            {
                // E-Commerce DbContext
                services.AddDbContext<ECommerceDbContext>(options =>
                    options.UseSqlServer(config.GetConnectionString("ECommerceConnection")));

                return services;
            }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"]; 
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("JWT Key not found in configuration!");
            }

            var jwtIssuer = configuration["Jwt:Issuer"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidIssuer = configuration["Jwt:Issuer"],
                     ValidAudience = configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                 };
             });

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddHttpClient<IPaymobService, PaymobService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
            services.AddScoped<ECommerceDbContext>();
            services.AddScoped<SeedInitialData>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<CartMappingProfile>();
            });
            services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
            return services;
        }

        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
            return services;
        } 
    }
}

