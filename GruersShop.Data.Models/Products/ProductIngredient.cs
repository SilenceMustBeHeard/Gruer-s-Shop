using GruersShop.Data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GruersShop.Data.Models.Products;

public class ProductIngredient : BaseEntity
{
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    public Guid IngredientId { get; set; }
    [ForeignKey(nameof(IngredientId))]
    public virtual Ingredient Ingredient { get; set; } = null!;

    [Range(0.1, 5000)]
    public double GramsUsed { get; set; }
}