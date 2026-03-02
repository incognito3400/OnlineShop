using Microsoft.EntityFrameworkCore;
using Exam2.Backend.Data;
using Exam2.Backend.Entities;
using Shop.Interfaces;
using Shop.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddControllers(); // Add Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Cloudinary Settings
builder.Services.Configure<Shop.Configuration.CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IImageService, CloudinaryService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    // TEMPORARY: Patch the existing database to add the ImageUrl to Users
    try
    {
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Users ADD COLUMN ImageUrl TEXT NULL;");
    }
    catch
    {
        // Column might already exist, ignore error
    }
}

app.MapControllers(); // Map Controllers

app.MapGet("/", () => "Shop API is running! Endpoints: /api/products, /api/categories, /api/orders, /api/auth/login, /api/auth/register, /swagger");

app.MapGet("/test-db", async (ApplicationDbContext context) =>
{
    try
    {
        var productsCount = await context.Products.CountAsync();
        var categoriesCount = await context.Categories.CountAsync();
        var usersCount = await context.Users.CountAsync();
        var ordersCount = await context.Orders.CountAsync();

        return Results.Ok(new
        {
            status = "success",
            database = "connected",
            counts = new { productsCount, categoriesCount, usersCount, ordersCount }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Database error: {ex.Message}");
    }
});

app.Run();