namespace E_commarce_Backend.Models.Support
{
    public class Conversation
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int ChannelId { get; set; }

        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Message> Messages { get; set; } = new();
    }
}
