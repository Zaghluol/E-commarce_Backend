using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Models.Support;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class SupportService(ECommerceDbContext context) : ISupportService
    {
        public async Task<List<SupportChannel>> GetChannelsAsync()
        {
            return await context.SupportChannels
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<List<ConversationDto>> GetUserConversationsAsync(string userId)
        {
            return await context.Conversations
                .Where(c => c.UserId == userId)
                .Include(c => c.Messages)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    IsClosed = c.IsClosed,
                    Messages = c.Messages.Select(m => new MessageDto
                    {
                        Content = m.Content,
                        IsFromAdmin = m.IsFromAdmin,
                        CreatedAt = m.CreatedAt
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task SendMessageAsync(string userId, SendMessageDto dto)
        {
            Conversation conversation;

            if (dto.ConversationId == null)
            {
                // ✅ Create new
                conversation = new Conversation
                {
                    UserId = userId,
                    ChannelId = dto.ChannelId
                };

                context.Conversations.Add(conversation);
                await context.SaveChangesAsync();
            }
            else
            {
                conversation = await context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == dto.ConversationId && c.UserId == userId);

                // 🔥 FIX: fallback to create instead of crash
                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        UserId = userId,
                        ChannelId = dto.ChannelId
                    };

                    context.Conversations.Add(conversation);
                    await context.SaveChangesAsync();
                }
            }

            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = userId,
                Content = dto.Content,
                IsFromAdmin = false
            };

            context.Messages.Add(message);
            await context.SaveChangesAsync();
        }
    }
}
