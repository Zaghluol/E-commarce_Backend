using E_commarce_Backend.Models.Enums;

public class AdminOrderDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } // ✅ enum
    public DateTime CreatedAt { get; set; }
}