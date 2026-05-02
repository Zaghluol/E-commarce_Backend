namespace E_commarce_Backend.Dtos.support
{
    public class ConversationDto
    {
        public int Id { get; set; }

        public bool IsClosed { get; set; }

        public List<MessageDto> Messages { get; set; }
    }
}
