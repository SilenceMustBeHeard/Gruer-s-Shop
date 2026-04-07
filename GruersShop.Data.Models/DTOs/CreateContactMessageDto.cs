using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.DTOs;

public record CreateContactMessageDto(
    [Required][MinLength(3)][MaxLength(100)] string Subject,
    [Required][MinLength(10)][MaxLength(5000)] string Message
);

public record CreateSystemMessageDto(
    [Required][MinLength(5)][MaxLength(200)] string Title,
    [Required][MinLength(20)][MaxLength(2000)] string Description,
    [Url] string? ActionUrl = null,
    DateTime? ExpiresAt = null,
    int Priority = 0
);

public record ContactMessageResponseDto(
    Guid Id,
    string UserId,
    string UserName,
    string Subject,
    string Message,
    DateTime CreatedAt,
    bool IsReadByAdmin,
    string? Response,
    DateTime? RespondedAt
);

public record SystemMessageResponseDto(
    Guid Id,
    string Title,
    string Description,
    string? ActionUrl,
    DateTime CreatedAt,
    bool IsRead,
    DateTime? ExpiresAt,
    int Priority
);