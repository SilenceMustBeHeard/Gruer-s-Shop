using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin;

public class ChangeUserRoleViewModel
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;


    public IEnumerable<string> AvailableRoles { get; set; } = new List<string>();
}
