using GruersShop.Web.ViewModels.Account.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Account;

public interface IProfileService
{

    Task<ProfileViewModel?> GetProfileAsync(string userId);







}