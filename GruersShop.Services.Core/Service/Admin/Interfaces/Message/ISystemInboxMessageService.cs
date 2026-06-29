using GruersShop.Data.Models.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using GruersShop.Web.ViewModels.Admin.Message;

namespace GruersShop.Services.Core.Service.Admin.Interfaces.Message;

public interface ISystemInboxMessageService
{
    Task MarkMessageAsReadAsync(Guid messageId, string userId);

    Task<int> GetUnreadCountAsync(string userId);

    Task<SystemInboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);

    Task<List<SystemInboxMessageViewModel>> GetAdminMessagesAsync(string adminId);

    Task CreateMessageAsync(SystemInboxMessage message);

    Task<List<SystemInboxMessageViewModel>> GetUserMessagesAsync(string userId);

    Task<SystemInboxMessageCreateViewModel> GetCreateViewModelAsync(string? userId = null);

    Task<(bool Success, string ErrorMessage)> CreateMessageAsync(
        SystemInboxMessageCreateViewModel model,
        string adminId);
}