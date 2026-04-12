using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Admin.Products;


    public class ProductViewModelList
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal Price { get; set; }
    
        public bool IsDeleted { get; set; }
    }

