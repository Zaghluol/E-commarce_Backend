namespace E_commarce_Backend.Models.Support
{
    public class SupportChannel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!; // e.g. "Chat", "Email"

        public bool IsActive { get; set; } = true;
    }
}
