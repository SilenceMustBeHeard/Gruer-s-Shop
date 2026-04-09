using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GruersShop.Data.Models.Interactions;

public class Review : BaseDeletableEntity
{
  

    public string UserId { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;

    
    [Range(0, 5)]
    public int Rating { get; set; }

    // Optional comment
    public string? Comment { get; set; }


    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;


}