using Microsoft.AspNetCore.SignalR;

namespace E_commarce_Backend.Dtos.support
{

    public class SupportHub : Hub
    {
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}
