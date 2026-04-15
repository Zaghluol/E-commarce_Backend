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
            modelBuilder.Entity<Address>()
            .HasIndex(a => a.UserId);
            modelBuilder.Entity<OrderStatusHistory>()
            .HasOne(h => h.Order)
            .WithMany(o => o.StatusHistory)
            .HasForeignKey(h => h.OrderId);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    }

}
