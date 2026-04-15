namespace E_commarce_Backend.Services.Abstractions
{
    public interface INotificationService
    {
        Task SendAsync(string userId, string title, string message);
    }
}
