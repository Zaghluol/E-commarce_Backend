namespace E_commarce_Backend.Dtos.support
{
    public class MessageDto
    {
        public string Content { get; set; }
        public bool IsFromAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
