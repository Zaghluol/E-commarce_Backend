namespace E_commarce_Backend.Dtos.Cart
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

}
