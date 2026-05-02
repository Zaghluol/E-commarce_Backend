namespace E_commarce_Backend.Models.Support
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string SenderId { get; set; } = null!; // user or admin

        public string Content { get; set; } = null!;

        public bool IsFromAdmin { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Conversation Conversation { get; set; } = null!;
    }
}
