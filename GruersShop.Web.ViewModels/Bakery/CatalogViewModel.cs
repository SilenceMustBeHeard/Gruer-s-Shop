using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Bakery;

public class CategoryNavViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? IconClass { get; set; }
    public int ProductCount { get; set; }
}