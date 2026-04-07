using GruersShop.Data.Models.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Admin.Interfaces.Message;

public interface ISystemInboxMessageService
{
    Task MarkMessageAsReadAsync(Guid messageId, string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task<SystemInboxMessageViewModel?> GetMessageDetailsAsync(Guid messageId, string userId);

    Task<List<SystemInboxMessageViewModel>> GetAdminMessagesAsync(string adminId);

    Task CreateMessageAsync(SystemInboxMessage message);
    Task<List<SystemInboxMessageViewModel>> GetUserMessagesAsync(string userId);

}