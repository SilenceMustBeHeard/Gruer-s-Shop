using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin
{
    public class UserManagmentIndexViewModel
    {
        public Guid Id { get; set; }

        // user email
        public string Email { get; set; } = null!;

        // list of roles (e.g., "User", "Admin")

        public IEnumerable<string> Roles { get; set; } = null!;


        // lockout (ban) end date

        public DateTimeOffset? LockoutEnd { get; set; }

        // check if the user is banned (soft deleted) by checking if LockoutEnd is in the future
        public bool IsBanned => LockoutEnd != null && LockoutEnd > DateTimeOffset.UtcNow;

    }
}
