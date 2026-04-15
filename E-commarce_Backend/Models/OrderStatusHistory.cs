namespace E_commarce_Backend.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string Status { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}