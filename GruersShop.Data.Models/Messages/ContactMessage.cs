using GruersShop.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.Messages;

public class ContactMessage : BaseMessage
{
   

    [Required]
    [MinLength(3, ErrorMessage = "Subject must be at least 3 characters long.")]
    [MaxLength(100, ErrorMessage = "Subject cannot exceed 100 characters.")]
    public required string Subject { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Message must be at least 10 characters long.")]
    [MaxLength(5000, ErrorMessage = "Message cannot exceed 5000 characters.")]
    public required string Message { get; set; }


    [MaxLength(5000, ErrorMessage = "Response cannot exceed 5000 characters.")]
    public string? Response { get; set; }


    public bool IsReadByAdmin { get; set; }
    public DateTime? ReadByAdminAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? RespondedById { get; set; }

    // Navigation 
    public virtual AppUser? RespondedBy { get; set; }

}