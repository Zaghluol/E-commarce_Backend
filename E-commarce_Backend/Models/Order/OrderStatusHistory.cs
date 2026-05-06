using E_commarce_Backend.Models.Enums;

public class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}