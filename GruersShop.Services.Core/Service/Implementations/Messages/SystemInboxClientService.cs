using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Implementations.Messages;

public class SystemInboxClientService : ISystemInboxClientService
{
    private readonly ISystemInboxMessageRepository _messageRepository;

    public SystemInboxClientService(ISystemInboxMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<SystemInboxMessageViewModel>> GetUserMessagesAsync(string userId)
    {
        return await _messageRepository
            .GetAllAttachedAsync()
            .Where(m => m.ReceiverId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new SystemInboxMessageViewModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                IsRead = m.IsRead,
                CreatedOn = m.CreatedAt,
                Type = m.Type
            })
            .ToListAsync();
    }

    public async Task<SystemInboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
    {
        var message = await _messageRepository
            .GetAllAttachedAsync()
            .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

        if (message == null)
            return null;

        if (!message.IsRead)
        {
            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
            await _messageRepository.SaveChangesAsync();
        }

        return new SystemInboxMessageViewModel
        {
            Id = message.Id,
            Title = message.Title,
            Description = message.Description,
            IsRead = message.IsRead,
            CreatedOn = message.CreatedAt,
            Type = message.Type
        };
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _messageRepository
            .GetAllAttachedAsync()
            .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
    }

    public async Task MarkAsReadAsync(Guid messageId, string userId)
    {
        var message = await _messageRepository
            .GetAllAttachedAsync()
            .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);

        if (message != null && !message.IsRead)
        {
            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
        }
    }
}