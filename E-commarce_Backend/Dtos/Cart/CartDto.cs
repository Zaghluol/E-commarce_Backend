namespace E_commarce_Backend.Dtos.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
