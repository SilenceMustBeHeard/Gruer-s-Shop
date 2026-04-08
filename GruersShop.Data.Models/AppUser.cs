
using GruersShop.Data.Models.Messages;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Data.Models;

public class AppUser : IdentityUser
{
    public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string AlternateEmail { get; set; } = null!;





    // System messages (SystemInboxMessage)
    public ICollection<SystemInboxMessage> ReceivedSystemMessages { get; set; }
        = new HashSet<SystemInboxMessage>();

    public ICollection<SystemInboxMessage> SentSystemMessages { get; set; }
        = new HashSet<SystemInboxMessage>();


    // Contact messages (ContactMessage)
    public ICollection<ContactMessage> ReceivedContactMessages { get; set; }
        = new HashSet<ContactMessage>();

    public ICollection<ContactMessage> SentContactMessages { get; set; }
        = new HashSet<ContactMessage>();
}
