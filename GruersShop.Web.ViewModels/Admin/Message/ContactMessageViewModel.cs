using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin.Message;

public class ContactMessageViewModel
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public bool IsReadByAdmin { get; set; }
    public string? Response { get; set; }
    public DateTime? RespondedOn { get; set; }
    public string? RespondedByAdminId { get; set; }
}