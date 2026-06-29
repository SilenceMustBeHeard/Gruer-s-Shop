using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.Messages;

public abstract class BaseMessage : BaseDeletableEntity
{
    [Required]
    public string ReceiverId { get; set; }

    public string? SenderId { get; set; }

    [Required]
    public InboxMessageType Type { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties - using required keyword (C# 11+)
    [Required]
    public virtual AppUser Receiver { get; set; }

    public virtual AppUser? Sender { get; set; }
}