using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Account.Messages;

public class ContactMessageDetailsViewModel
{
    public Guid Id { get; set; }

    // From the submitted form
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;


    // Sender info
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;

    // Receiver info
    public string ReceiverName { get; set; } = null!;
    public string ReceiverEmail { get; set; } = null!;

    // Status information
    public bool IsReadByAdmin { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedOn { get; set; }

    // Response info
    public string? Response { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? RespondedByName { get; set; }

    // For UI display formatting
    public string CreatedOnFormatted
        => CreatedOn.ToString("dd MMM yyyy HH:mm");

    public string? RespondedOnFormatted
        => RespondedAt?.ToString("dd MMM yyyy HH:mm");
}