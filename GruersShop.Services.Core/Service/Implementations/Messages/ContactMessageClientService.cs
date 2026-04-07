using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GruersShop.Services.Core.Service.Implementations.Messages;

public class ContactMessageClientService : IContactMessageClientService
{
    private readonly IContactMessageRepository _messageRepository;
    private readonly UserManager<AppUser> _userManager;

    public ContactMessageClientService(
        IContactMessageRepository messageRepository,
        UserManager<AppUser> userManager)
    {
        _messageRepository = messageRepository;
        _userManager = userManager;
    }

    public async Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal)
    {
        var sender = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new ArgumentException("You must be logged in to send a contact message.");

        var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault()
            ?? throw new InvalidOperationException("No admin user found in the system.");

      
        var existingMessage = await _messageRepository
            .Query()
            .FirstOrDefaultAsync(m => m.SenderId == sender.Id
                && m.Subject == model.Subject
                && m.Message == model.Message
                && m.ReceiverId == adminUser.Id
                && !m.IsDeleted);

        if (existingMessage == null)
        {
            var contactMessage = new ContactMessage
            {
                Id = Guid.NewGuid(),
                SenderId = sender.Id,
                ReceiverId = adminUser.Id,
                Receiver = adminUser,
                Subject = model.Subject,
                Message = model.Message,
                Type = InboxMessageType.UserToAdmin,
                IsRead = false,
                IsReadByAdmin = false,
                CreatedAt = DateTime.UtcNow  
            };

            await _messageRepository.AddAsync(contactMessage);
            await _messageRepository.SaveChangesAsync();
        }
    }

    public async Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId)
    {
        var messages = await _messageRepository
            .Query()
            .Where(m => m.SenderId == userId && !string.IsNullOrEmpty(m.Response) && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        // Load users separately since navigation properties might not be loaded
        var user = await _userManager.FindByIdAsync(userId);
        var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();

        return messages.Select(m => new ContactMessageDetailsViewModel
        {
            Id = m.Id,
            Subject = m.Subject,
            Message = m.Message,
            SenderName = user?.FullName ?? "Unknown",
            SenderEmail = user?.Email ?? string.Empty,
            ReceiverName = "Admin",
            ReceiverEmail = adminUser?.Email ?? "admin@gruersshop.com",
            IsRead = m.IsRead,
            IsReadByAdmin = m.IsReadByAdmin,
            CreatedOn = m.CreatedAt,
            Response = m.Response,
            RespondedAt = m.RespondedAt,
            RespondedByName = "Admin" // You'd need to load the admin who responded
        }).ToList();
    }

    public async Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId)
    {
        var message = await _messageRepository
            .Query()
            .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId && !m.IsDeleted);

        if (message == null) return null;

        // Mark as read if there's a response and it hasn't been read
        if (!string.IsNullOrEmpty(message.Response) && !message.IsRead)
        {
            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
            await _messageRepository.SaveChangesAsync();
        }

        var user = await _userManager.FindByIdAsync(userId);
        var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();

        return new ContactMessageDetailsViewModel
        {
            Id = message.Id,
            Subject = message.Subject,
            Message = message.Message,
            SenderName = user?.FullName ?? "Unknown",
            SenderEmail = user?.Email ?? string.Empty,
            ReceiverName = "Admin",
            ReceiverEmail = adminUser?.Email ?? "admin@gruersshop.com",
            IsRead = message.IsRead,
            IsReadByAdmin = message.IsReadByAdmin,
            CreatedOn = message.CreatedAt,
            Response = message.Response,
            RespondedAt = message.RespondedAt,
            RespondedByName = "Admin" // You'd need to load the admin who responded
        };
    }

    public async Task<int> GetUserUnreadResponsesCountAsync(string userId)
    {
        return await _messageRepository
            .Query()
            .CountAsync(m => m.SenderId == userId
                && !string.IsNullOrEmpty(m.Response)
                && !m.IsRead
                && !m.IsDeleted);
    }
}