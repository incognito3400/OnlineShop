using Microsoft.EntityFrameworkCore;
using Exam2.Backend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Database testing endpoint
app.MapGet("/test-db", async (ApplicationDbContext context) =>
{
    try
    {
        // Ensure database exists (for dev only)
        await context.Database.EnsureCreatedAsync();

        // OPERATION 1: Get products by filter (price > 100)
        var expensiveProducts = await context.Products
            .Where(p => p.Price > 100)
            .Select(p => new { p.Id, p.Name, p.Price })
            .ToListAsync();

        // OPERATION 2: Get full product data with related category
        var productWithDetails = await context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == 1)
            .Select(p => new
            {
                p.Name,
                Category = p.Category != null ? p.Category.Name : null
            })
            .FirstOrDefaultAsync();

        return Results.Ok(new
        {
            status = "success",
            expensiveProductsCount = expensiveProducts.Count,
            expensiveProducts,
            productWithDetails
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Database error: {ex.Message}");
    }
});

app.MapGet("/", () => "Hello World! Use /test-db to test database.");

app.Run();