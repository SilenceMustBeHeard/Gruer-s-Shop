using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.Messages;

public class SystemInboxMessage : BaseMessage
{
    [Required]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public required string Title { get; set; }

    [Required]
    [MinLength(20, ErrorMessage = "Description must be at least 20 characters long.")]
    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public required string Description { get; set; }

    [Url(ErrorMessage = "Action URL must be a valid URL.")]
    public string? ActionUrl { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int Priority { get; set; } = 0;
}