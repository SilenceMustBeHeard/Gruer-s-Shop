using GruersShop.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Web.ViewModels.Account.Messages;

public class SystemInboxMessageViewModel
{
    public Guid Id { get; set; }
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }
    [Required]
    [MinLength(20, ErrorMessage = "Description must be at least 20 characters long.")]
    public string Description { get; set; } = null!;
    public bool IsRead { get; set; }

    public DateTime CreatedOn { get; set; }

    public InboxMessageType Type { get; set; }
    public string? TypeDisplayName => Type.ToString();
    public string? SenderName { get; set; }
    public string? ReceiverName { get; set; }
}