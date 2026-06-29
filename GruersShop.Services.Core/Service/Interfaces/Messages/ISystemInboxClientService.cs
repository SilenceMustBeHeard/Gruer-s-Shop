using GruersShop.Web.ViewModels.Account.Messages;

namespace GruersShop.Services.Core.Service.Interfaces.Messages;

public interface ISystemInboxClientService
{
    Task<List<SystemInboxMessageViewModel>> GetUserMessagesAsync(string userId);

    Task<SystemInboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);

    Task<int> GetUnreadCountAsync(string userId);

    Task MarkAsReadAsync(Guid messageId, string userId);
}