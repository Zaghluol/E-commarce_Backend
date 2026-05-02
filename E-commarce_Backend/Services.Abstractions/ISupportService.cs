using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Models.Support;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface ISupportService
    {
        Task<List<SupportChannel>> GetChannelsAsync();

        Task<List<ConversationDto>> GetUserConversationsAsync(string userId);

        Task SendMessageAsync(string userId, SendMessageDto dto);
    }
}
