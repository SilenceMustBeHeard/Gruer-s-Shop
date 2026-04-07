using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Models;

public abstract class BaseDeletableEntity : BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
