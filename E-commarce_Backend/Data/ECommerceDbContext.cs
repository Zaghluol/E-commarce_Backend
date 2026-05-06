using E_commarce_Backend.Models;
using E_commarce_Backend.Models.Nofications;
using E_commarce_Backend.Models.order;
using E_commarce_Backend.Models.Support;
using E_commarce_Backend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Data
{
    public class ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Favorite>()
               .HasIndex(f => new { f.UserId, f.ProductId })  
               .IsUnique();
            modelBuilder.Entity<Address>()
            .HasIndex(a => a.UserId);
            modelBuilder.Entity<NotificationSettings>()
            .HasIndex(ns => ns.UserId)
            .IsUnique(); // each user has only one settings row
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();
            modelBuilder.Entity<OrderStatusHistory>()
                .Property(o => o.Status)
                .HasConversion<string>();
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
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<SupportChannel> SupportChannels { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
    }

}
