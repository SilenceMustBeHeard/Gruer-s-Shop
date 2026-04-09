using GruersShop.Data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Data.Models.Catalog;

public class Catalog : BaseDeletableEntity
{
    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;

    // Navigation - One Catalog has many Categories
    public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
}