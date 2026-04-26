using GruersShop.Data.Models.Base;
using System.ComponentModel.DataAnnotations;
using GruersShop.Data.Common.Enums;

namespace GruersShop.Data.Models.Interactions;


public class Order : BaseDeletableEntity
{
    [Required]
    public string UserId { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;

    public DateTime? OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? PickupDate { get; set; }

    [Range(0, 1000)]
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string? SpecialInstructions { get; set; }
    public string? PickupTime { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}