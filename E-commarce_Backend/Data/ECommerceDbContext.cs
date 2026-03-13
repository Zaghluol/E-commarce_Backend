using E_commarce_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Data
{
    public class ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorite>()
               .HasIndex(f => new { f.UserId, f.ProductId })
               .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

    }

}
