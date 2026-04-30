using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using GruersShop.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Web.Controllers.Account;

public class HomeController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public HomeController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            TopRatedProducts = await _productRepository
                .Query()
                .Where(p => !p.IsDeleted && p.IsAvailable)
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.Reviews.Count())
                .Take(3)
                .ToListAsync(),

            Categories = await _categoryRepository
                .Query()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync()
        };

        return View(viewModel);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode.HasValue)
        {
            ViewBag.StatusCode = statusCode.Value;
        }
        return View();
    }
}