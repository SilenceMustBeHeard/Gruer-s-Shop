using GruersShop.Web.ViewModels.Account.Messages;
using GruersShop.Web.ViewModels.Admin.Message;
using System.Security.Claims;

namespace GruersShop.Services.Core.Service.Interfaces.Messages;

public interface IContactMessageClientService
{
    Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal);

    Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId);

    Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);

    Task<int> GetUserUnreadResponsesCountAsync(string userId);

    Task<bool?> MarkAsReadAsync(Guid messageId, string userId);
}