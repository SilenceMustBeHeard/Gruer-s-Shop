using GruersShop.Web.ViewModels.Account.Messages;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Messages
{
    public interface IContactMessageClientService
    {
        Task SendContactMessageAsync(ContactMessageCreateViewModel model, ClaimsPrincipal userPrincipal);
        Task<List<ContactMessageDetailsViewModel>> GetUserMessagesAsync(string userId);
        Task<ContactMessageDetailsViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);
        Task<int> GetUserUnreadResponsesCountAsync(string userId);

    }
}
