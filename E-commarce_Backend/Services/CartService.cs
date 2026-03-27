using AutoMapper;
using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Cart;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class CartService(ECommerceDbContext context, IMapper mapper) : ICartService
    {
        // 🛒 Get or create cart
        public async Task<Cart> GetOrCreateCart(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var cart = await context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product) // ⭐ FIX: load product
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };

                context.Carts.Add(cart);
                await context.SaveChangesAsync();
            }

            return cart;
        }

        // 📦 Get cart
        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await GetOrCreateCart(userId);
            return mapper.Map<CartDto>(cart);
        }

        // ➕ Add to cart
        public async Task<CartDto> AddToCartAsync(string userId, int productId, int quantity)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var cart = await GetOrCreateCart(userId);

            var productExists = await context.Products
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
                throw new KeyNotFoundException("Product not found");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await context.SaveChangesAsync();

            // ⭐ return fresh updated cart
            return await GetCartAsync(userId);
        }

        // ✏️ Update item
        public async Task UpdateItemAsync(string userId, int itemId, int quantity)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var item = await context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i =>
                    i.Id == itemId &&
                    i.Cart.UserId == userId);

            if (item == null)
                throw new KeyNotFoundException("Cart item not found");

            if (quantity <= 0)
                context.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            await context.SaveChangesAsync();
        }

        // ❌ Remove item
        public async Task RemoveItemAsync(string userId, int itemId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var item = await context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i =>
                    i.Id == itemId &&
                    i.Cart.UserId == userId);

            if (item == null)
                throw new KeyNotFoundException("Cart item not found");

            context.CartItems.Remove(item);
            await context.SaveChangesAsync();
        }

        // 🧹 Clear cart
        public async Task ClearCartAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var cart = await GetOrCreateCart(userId);

            context.CartItems.RemoveRange(cart.Items);

            await context.SaveChangesAsync();
        }
    }
}