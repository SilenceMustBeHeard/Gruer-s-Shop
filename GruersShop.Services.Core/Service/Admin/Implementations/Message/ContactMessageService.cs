using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Account.Messages;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Message;

public class ContactMessageService : IContactMessageService
{
    private readonly IContactMessageRepository _messageRepository;

    public ContactMessageService(IContactMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<ContactMessageDetailsViewModel>> GetAdminMessagesAsync(string adminId)
    {
        return await _messageRepository
            .GetAllAttachedAsync()
            .Include(m => m.Sender)
            .Include(m => m.RespondedBy)
            .Where(m => m.ReceiverId == adminId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new ContactMessageDetailsViewModel
            {
                Id = m.Id,
                Subject = m.Subject,
                Message = m.Message,
                SenderName = m.Sender!.FullName ?? "Unknown",
                SenderEmail = m.Sender!.Email ?? string.Empty,
                IsRead = m.IsRead,
                IsReadByAdmin = m.IsReadByAdmin,
                CreatedOn = m.CreatedAt,
                Response = m.Response,
                RespondedAt = m.RespondedAt,
                RespondedByName = m.RespondedBy!.FullName
            })
            .ToListAsync();
    }

    public async Task RespondToMessageAsync(Guid messageId, string response, string adminId)
    {
        var message = await _messageRepository
            .FirstOrDefaultAsync(m => m.Id == messageId)
            ?? throw new ArgumentException("Message not found");

        if (!string.IsNullOrEmpty(message.Response))
        {
            throw new InvalidOperationException("This message has already been responded to.");
        }

        message.Response = response;
        message.RespondedAt = DateTime.UtcNow;
        message.RespondedById = adminId;
        message.IsReadByAdmin = true;

        await _messageRepository.UpdateAsync(message);
    }

    public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string adminId)
    {
        var message = await _messageRepository
            .GetAllAttachedAsync()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.RespondedBy)
            .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == adminId);

        if (message == null) return null;


        if (!message.IsReadByAdmin)
        {
            message.IsReadByAdmin = true;
            await _messageRepository.UpdateAsync(message);
        }

        return new ContactMessageDetailsViewModel
        {
            Id = message.Id,
            Subject = message.Subject,
            Message = message.Message,
            SenderName = message.Sender!.FullName ?? "Unknown",
            SenderEmail = message.Sender!.Email ?? string.Empty,
            ReceiverName = message.Receiver!.FullName ?? "Admin",
            ReceiverEmail = message.Receiver!.Email ?? string.Empty,
            IsRead = message.IsRead,
            IsReadByAdmin = message.IsReadByAdmin,
            CreatedOn = message.CreatedAt,
            Response = message.Response,
            RespondedAt = message.RespondedAt,
            RespondedByName = message.RespondedBy?.FullName
        };
    }

    public async Task<int> GetUnreadCountAsync(string adminId)
    {
        return await _messageRepository
            .GetAllAttachedAsync()
            .CountAsync(m => m.ReceiverId == adminId && !m.IsReadByAdmin);
    }

    public async Task MarkMessageAsReadAsync(Guid messageId, string adminId)
    {
        var message = await _messageRepository
            .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == adminId);

        if (message != null && !message.IsReadByAdmin)
        {
            message.IsReadByAdmin = true;
            await _messageRepository.UpdateAsync(message);
        }
    }

}