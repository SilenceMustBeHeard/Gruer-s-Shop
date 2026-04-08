
using GruersShop.Web.ViewModels.Account.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.User
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;

        public string? Address { get; set; }

   
        public IEnumerable<SystemInboxMessageViewModel> SystemInbox { get; set; } 
            = new List<SystemInboxMessageViewModel>();

        public IEnumerable<ContactMessageDetailsViewModel> ContactMessages { get; set; }
            = new List<ContactMessageDetailsViewModel>();
    }

}
