using GruersShop.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Data.Models.Messages;

public abstract class BaseMessage : BaseDeletableEntity
{
    [Required]
    public required string ReceiverId { get; set; }

    public string? SenderId { get; set; }

    [Required]
    public InboxMessageType Type { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties - using required keyword (C# 11+)
    [Required]
    public virtual required AppUser Receiver { get; set; }

    public virtual AppUser? Sender { get; set; }
}
