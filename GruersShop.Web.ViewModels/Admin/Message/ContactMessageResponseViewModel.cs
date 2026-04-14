using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin.Message;

public class ContactMessageResponseViewModel
{
    public Guid Id { get; set; }


    public string Subject { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string OriginalMessage { get; set; } = null!;


    [Required(ErrorMessage = "Please enter a response.")]
    [MaxLength(5000, ErrorMessage = "Response cannot exceed 5000 characters.")]
    public string Response { get; set; } = null!;
}