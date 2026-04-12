using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin.Category;

public class CategoryViewModelList
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }
}
