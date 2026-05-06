using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Models.Support;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class SupportService(ECommerceDbContext context,
        INotificationService notificationService,
        IHubContext<SupportHub> hub) : ISupportService
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

            // 1️⃣ Create or get conversation
            if (dto.ConversationId == null)
            {
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

                // 🔥 Fix: fallback create instead of crash
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

            // 2️⃣ Create message
            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = userId,
                Content = dto.Content,
                IsFromAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Messages.Add(message);
            await context.SaveChangesAsync();

            // 3️⃣ 🔴 Real-time update (SignalR)
            await hub.Clients
                .Group(conversation.Id.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    conversationId = conversation.Id,
                    content = message.Content,
                    isFromAdmin = false,
                    createdAt = message.CreatedAt
                });

            // 4️⃣ 🔔 Notify admin (optional)
            await notificationService.SendAsync(
                "ADMIN",
                "New Support Message",
                message.Content
            );
        }
        public async Task AdminReplyAsync(AdminReplyDto dto)
        {
            // 1️⃣ Get conversation
            var conversation = await context.Conversations
                .FirstOrDefaultAsync(c => c.Id == dto.ConversationId);

            if (conversation == null)
                throw new Exception("Conversation not found");

            // 2️⃣ Create message
            var message = new Message
            {
                ConversationId = dto.ConversationId,
                SenderId = "ADMIN",
                Content = dto.Content,
                IsFromAdmin = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Messages.Add(message);
            await context.SaveChangesAsync();

            // 3️⃣ 🔴 Real-time update (SignalR)
            await hub.Clients
                .Group(dto.ConversationId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    conversationId = dto.ConversationId,
                    content = message.Content,
                    isFromAdmin = true,
                    createdAt = message.CreatedAt
                });

            // 4️⃣ 🔔 Notify user
            await notificationService.SendAsync(
                conversation.UserId,
                "Support Reply",
                message.Content
            );
        }
    }
}
