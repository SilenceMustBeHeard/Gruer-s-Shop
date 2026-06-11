using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Implementations.Account;
using GruersShop.Data.Repositories.Interfaces.Account;
using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Account.Messages;
using GruersShop.Web.ViewModels.Admin.Message;
using GruersShop.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Message;

public class SystemInboxMessageService : ISystemInboxMessageService

{
    private readonly ISystemInboxMessageRepository _messageRepository;
    private readonly IAppUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SystemInboxMessageService(ISystemInboxMessageRepository messageRepository,
        UserManager<AppUser> userManager,
         IAppUserRepository userRepository,
        RoleManager<IdentityRole> roleManager)
    {
        _messageRepository = messageRepository;
        _userManager = userManager;
        _userRepository = userRepository;
        _roleManager = roleManager;
    }

    public async Task<List<SystemInboxMessageViewModel>> GetAdminMessagesAsync(string adminId)
    {
        return await _messageRepository
            .GetAllAttachedAsync()
            .Where(m => m.ReceiverId == adminId)
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
            message.ReadAt = DateTime.UtcNow;
            await _messageRepository.UpdateAsync(message);
            await _messageRepository.SaveChangesAsync();
        }

        var sender = await _userManager.FindByIdAsync(message.SenderId ?? "");
        var receiver = await _userManager.FindByIdAsync(message.ReceiverId!);

        return new SystemInboxMessageViewModel
        {
            Id = message.Id,
            Title = message.Title,
            Description = message.Description,
            IsRead = message.IsRead,
            CreatedOn = message.CreatedAt,
            Type = message.Type,
            SenderId = message.SenderId,
            SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "System",
            ReceiverName = receiver != null ? $"{receiver.FirstName} {receiver.LastName}" : "Unknown"
        };
    }

    public async Task<SystemInboxMessageCreateViewModel> GetCreateViewModelAsync(string? userId = null)
    {
        var users = await _userRepository
            .Query()
        .Where(u => u.IsActive)
            .Select(u => new UserSelectViewModel
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email ?? string.Empty
            })
            .ToListAsync();

        var model = new SystemInboxMessageCreateViewModel
        {
            AvailableUsers = users,
            ReceiverId = userId,
            Title = string.Empty,
            Description = string.Empty,
        };

        if (!string.IsNullOrEmpty(userId))
        {
            var selectedUser = users.FirstOrDefault(u => u.Id == userId);
            if (selectedUser != null)
            {
                model.ReceiverName = selectedUser.FullName;
            }
        }

        return model;
    }

    public async Task<(bool Success, string ErrorMessage)> CreateMessageAsync(
        SystemInboxMessageCreateViewModel model,
        string adminId)
    {

        if (string.IsNullOrEmpty(model.ReceiverId))
        {
            return (false, "Please select a receiver.");
        }

        var receiver = await _userManager.FindByIdAsync(model.ReceiverId);
        if (receiver == null)
        {
            return (false, "Receiver not found.");
        }

        var message = new SystemInboxMessage
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Description = model.Description,
            Type = model.Type,
            SenderId = adminId,
            ReceiverId = model.ReceiverId,
            Receiver = receiver,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangesAsync();

        return (true, string.Empty);
    }
    public async Task MarkMessageAsReadAsync(Guid messageId, string userId)
    {
        var message = await _messageRepository
            .FirstOrDefaultAsync(x => x.Id == messageId && x.ReceiverId == userId);

        if (message == null)
            return;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        await _messageRepository.UpdateAsync(message);
        await _messageRepository.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _messageRepository
            .GetAllAttachedAsync()
            .CountAsync(x => x.ReceiverId == userId && !x.IsRead);
    }

    public async Task CreateMessageAsync(SystemInboxMessage message)
    {
        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangesAsync();
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
                Type = m.Type,
                SenderId = m.SenderId
            })
            .ToListAsync();
    }
}
