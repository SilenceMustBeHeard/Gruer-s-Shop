using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Products;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GruersShop.Data.Seeding;

public static class DbSeeder
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public static async Task SeedCatalogsAsync(AppDbContext context)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (await context.Catalogs.AnyAsync())
            {
                Console.WriteLine("Catalogs already exist. Skipping.");
                return;
            }

            var jsonPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "data",
                "catalogs.json"
            );

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"catalogs.json NOT FOUND at: {jsonPath}");
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var catalogs = JsonSerializer.Deserialize<List<Catalog>>(json, options)
                ?? throw new Exception("catalogs.json is empty or invalid");

            foreach (var catalog in catalogs)
            {
                if (string.IsNullOrWhiteSpace(catalog.Name))
                    throw new Exception("Catalog Name is NULL or EMPTY");

                if (catalog.Id == Guid.Empty)
                    catalog.Id = Guid.NewGuid();

                catalog.IsDeleted = false;
                catalog.IsActive = true;
            }

            await context.Catalogs.AddRangeAsync(catalogs);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {catalogs.Count} catalogs from file.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public static async Task SeedCategoriesAsync(AppDbContext context)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (await context.Categories.AnyAsync())
            {
                Console.WriteLine("Categories already exist. Skipping.");
                return;
            }

            var jsonPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "data",
                "categories.json"
            );

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"categories.json NOT FOUND at: {jsonPath}");
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var categories = JsonSerializer.Deserialize<List<Category>>(json, options)
                ?? throw new Exception("categories.json is empty or invalid");

            foreach (var category in categories)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                    throw new Exception("Category Name is NULL or EMPTY");

                if (category.Id == Guid.Empty)
                    category.Id = Guid.NewGuid();

                var catalogExists = await context.Catalogs.AnyAsync(c => c.Id == category.CatalogId);
                if (!catalogExists)
                {
                    throw new Exception($"CatalogId {category.CatalogId} does not exist for category: {category.Name}");
                }

                category.IsDeleted = false;
            }

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {categories.Count} categories from file.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public static async Task SeedProductsAsync(AppDbContext context)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (await context.Products.AnyAsync())
            {
                Console.WriteLine("Products already exist. Skipping.");
                return;
            }

            var jsonPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "data",
                "products.json"
            );

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"products.json NOT FOUND at: {jsonPath}");
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var products = JsonSerializer.Deserialize<List<Product>>(json, options)
                ?? throw new Exception("products.json is empty or invalid");

            var categories = await context.Categories.ToDictionaryAsync(c => c.Id, c => c.Name);

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new Exception("Product Name is NULL or EMPTY");

                if (string.IsNullOrWhiteSpace(product.Description))
                    throw new Exception($"Product Description is NULL for: {product.Name}");

                if (string.IsNullOrWhiteSpace(product.ImageUrl))
                    throw new Exception($"Product ImageUrl is NULL for: {product.Name}");

                if (product.Price <= 0)
                    throw new Exception($"Product Price is INVALID for: {product.Name}");

                if (!categories.ContainsKey(product.CategoryId))
                    throw new Exception($"CategoryId {product.CategoryId} does not exist for product: {product.Name}");

                if (product.Id == Guid.Empty)
                    product.Id = Guid.NewGuid();

                product.IsAvailable = true;
                product.IsDeleted = false;
                product.AverageRating = 0;
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {products.Count} products from file.");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public static async Task SeedAllAsync(AppDbContext context)
    {
        Console.WriteLine("🌱 Starting database seeding from files...");

        await SeedCatalogsAsync(context); await SeedCategoriesAsync(context); await SeedProductsAsync(context);
        Console.WriteLine("✅ Database seeding completed!");
    }
}