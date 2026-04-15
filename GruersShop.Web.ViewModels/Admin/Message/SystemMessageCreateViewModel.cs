using GruersShop.Data.Common.Enums;
using GruersShop.Web.ViewModels.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin.Message;

public class SystemInboxMessageCreateViewModel
{
    [Required(ErrorMessage = "Please select a recipient")]
    public string ReceiverId { get; set; } = null!;
    public string? ReceiverName { get; set; }

    [Required]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
    [Display(Name = "Title")]
    public string Title { get; set; } = null!;


    [Required]
    [MinLength(20, ErrorMessage = "Description must be at least 20 characters long.")]
    [Display(Name = "Message")]
    public string Description { get; set; } = null!;

    [Required]
    [Display(Name = "Message Type")]
    public InboxMessageType Type { get; set; }

    public List<UserSelectViewModel> AvailableUsers { get; set; } = new();
}