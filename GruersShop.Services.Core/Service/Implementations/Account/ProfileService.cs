using GruersShop.Data.Models.Base;
using GruersShop.Data.Repositories.Implementations.Account;
using GruersShop.Data.Repositories.Interfaces.Account;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Implementations.Account;

public class ProfileService : IProfileService
{
    private readonly IAppUserRepository _userRepository;  
    private readonly UserManager<AppUser> _userManager;
    private readonly ISystemInboxMessageService _systemInboxMessageService;
    private readonly IContactMessageClientService _contactMessageClientService;
    private readonly IContactMessageService _contactMessageService;

    public ProfileService(
        IAppUserRepository userRepository,  
        UserManager<AppUser> userManager,
        ISystemInboxMessageService systemInboxMessageService,
        IContactMessageClientService contactMessageClientService,
        IContactMessageService contactMessageService)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _systemInboxMessageService = systemInboxMessageService;
        _contactMessageClientService = contactMessageClientService;
        _contactMessageService = contactMessageService;
    }
    public async Task<ProfileViewModel?> GetProfileAsync(string userId)
    {
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

      
        var systemMessages = await _systemInboxMessageService.GetUserMessagesAsync(userId);

        var roles = await _userManager.GetRolesAsync(user);
        var isAdmin = roles.Contains("Admin");
        var isManager = roles.Contains("Manager");

        List<ContactMessageDetailsViewModel> contactMessages = new List<ContactMessageDetailsViewModel>();

        if (isAdmin)
        {

            contactMessages = await _contactMessageService.GetAdminMessagesAsync(userId);
        }
        else if (!isManager)
        {

            contactMessages = await _contactMessageClientService.GetUserMessagesAsync(userId);
        }


        return new ProfileViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
         
            SystemInbox = systemMessages?.ToList() ?? new List<SystemInboxMessageViewModel>(),
            ContactMessages = contactMessages ?? new List<ContactMessageDetailsViewModel>()
        };
    }
}