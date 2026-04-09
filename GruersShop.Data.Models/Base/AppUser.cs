using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Messages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.Base;

public class AppUser : IdentityUser
{
    public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? AlternateEmail { get; set; }


    // Navigation properties for messages
    public virtual ICollection<SystemInboxMessage> ReceivedSystemMessages { get; set; }
        = new HashSet<SystemInboxMessage>();

    public virtual ICollection<SystemInboxMessage> SentSystemMessages { get; set; } 
        = new HashSet<SystemInboxMessage>();

    public virtual ICollection<ContactMessage> ReceivedContactMessages { get; set; } 
        = new HashSet<ContactMessage>();

    public virtual ICollection<ContactMessage> SentContactMessages { get; set; }
        = new HashSet<ContactMessage>();




    // Navigation properties for catalog
    public virtual ICollection<Favorite> Favorites { get; set; } 
        = new HashSet<Favorite>();

    public virtual ICollection<Review> Reviews { get; set; }
        = new HashSet<Review>();

    public virtual ICollection<Order> Orders { get; set; }
        = new HashSet<Order>();
}






