using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Products;

namespace GruersShop.Data.Models.Interactions;

public class Favorite : BaseDeletableEntity
{
    public string UserId { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;

    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}