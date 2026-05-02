namespace E_commarce_Backend.Dtos.support
{
    public class SendMessageDto
    {
        public int? ConversationId { get; set; } // null = new conversation
        public int ChannelId { get; set; }

        public string Content { get; set; } = null!;
    }
}
