using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Web.ViewModels.Account.Messages
{
    public class ContactMessageCreateViewModel
    {

        [Required(ErrorMessage = "Subject is required.")]
        [MinLength(3, ErrorMessage = "Subject must be at least 3 characters long.")]
        [MaxLength(50, ErrorMessage = "Subject cannot exceed 50 characters.")]
        public string Subject { get; set; } = null!;
        [Required(ErrorMessage = "Message is required.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters long.")]
        [MaxLength(5000, ErrorMessage = "Message cannot exceed 5000 characters.")]
        public string Message { get; set; } = null!;
    }
}
