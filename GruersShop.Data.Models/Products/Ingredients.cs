using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace GruersShop.Data.Models.Products;

[Table("Ingredients")]
public class Ingredient 
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    // Basic info
    public string? Description { get; set; }
    public string? Supplier { get; set; }
    public string? Origin { get; set; } // "Local", "Imported", etc.

    // Quality flags
    public bool IsOrganic { get; set; }
    public bool IsNonGMO { get; set; }
    public bool IsSustainable { get; set; }

    // Seasonality
    public string? BestSeason { get; set; } // "Spring", "Summer", "Fall", "Winter", "Year-round"

    // Common macros (always tracked)
    [Range(0, 1000)]
    public double CaloriesPer100g { get; set; }

    [Range(0, 100)]
    public double ProteinPer100g { get; set; }

    [Range(0, 100)]
    public double CarbsPer100g { get; set; }

    [Range(0, 100)]
    public double FatPer100g { get; set; }

    [Range(0, 100)]
    public double FiberPer100g { get; set; }

    [Range(0, 100)]
    public double SugarPer100g { get; set; }


    // Allergens
    public bool ContainsGluten { get; set; }
    public bool ContainsDairy { get; set; }
    public bool ContainsEggs { get; set; }
    public bool ContainsNuts { get; set; }
    public bool ContainsSoy { get; set; }
    public bool ContainsSesame { get; set; }



    // Store additional vitamins/minerals as JSON (flexible)
    [Column(TypeName = "nvarchar(max)")]
    public string AdditionalNutrientsJson { get; set; } = "{}";

    [NotMapped]
    public Dictionary<string, NutrientInfo> AdditionalNutrients
    {
        get => string.IsNullOrEmpty(AdditionalNutrientsJson)
            ? new Dictionary<string, NutrientInfo>()
            : JsonSerializer.Deserialize<Dictionary<string, NutrientInfo>>(AdditionalNutrientsJson) ?? new();
        set => AdditionalNutrientsJson = JsonSerializer.Serialize(value);
    }

    // Navigation
    public virtual ICollection<ProductIngredient> ProductIngredients { get; set; } = new HashSet<ProductIngredient>();
}

public class NutrientInfo
{
    public double AmountPer100g { get; set; }
    public string Unit { get; set; } = "mg";
    public double? DailyValuePercentage { get; set; }
}