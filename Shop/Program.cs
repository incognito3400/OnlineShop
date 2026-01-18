using Microsoft.EntityFrameworkCore;
using Exam2.Backend.Data;
using Exam2.Backend.Entities;
using Shop.Interfaces;
using Shop.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

// PRODUCTS ENDPOINTS

// GET /api/products - Загальний каталог
app.MapGet("/api/products", (IProductsService productsService, string? search, int? categoryId, decimal? minPrice, decimal? maxPrice) =>
{
    var products = productsService.GetProductsByFilter(search, minPrice, maxPrice, categoryId);
    return Results.Ok(products.Select(p => new
    {
        p.Id,
        p.Name,
        p.Price,
        p.OldPrice,
        p.ImageUrl,
        p.StockQuantity,
        p.CategoryId,
        CategoryName = p.Category?.Name
    }));
});

// GET /api/products/{id} - Сторінка товару (з деталями)
app.MapGet("/api/products/{id:int}", (int id, IProductsService productsService) =>
{
    var product = productsService.GetProductById(id);
    if (product == null) return Results.NotFound();
    
    return Results.Ok(new
    {
        product.Id,
        product.Name,
        product.Price,
        product.OldPrice,
        product.ImageUrl,
        product.StockQuantity,
        product.CategoryId,
        product.Description,
        Details = product.Details?.Select(d => new { d.Key, d.Value }) ?? Enumerable.Empty<object>()
    });
});

// GET /api/products/promotions - Акційні товари (OldPrice > Price)
app.MapGet("/api/products/promotions", (IProductsService productsService) =>
{
    var products = productsService.GetAllProducts()
        .Where(p => p.OldPrice.HasValue && p.OldPrice > p.Price);
    return Results.Ok(products.Select(p => new
    {
        p.Id,
        p.Name,
        p.Price,
        p.OldPrice,
        p.ImageUrl,
        Discount = p.OldPrice.HasValue ? Math.Round((1 - p.Price / p.OldPrice.Value) * 100) : 0
    }));
});

// GET /api/products/new - Новинки (останні 10, сортовані за CreatedAt DESC)
app.MapGet("/api/products/new", (IProductsService productsService) =>
{
    var products = productsService.GetAllProducts()
        .OrderByDescending(p => p.CreatedAt)
        .Take(10);
    return Results.Ok(products.Select(p => new
    {
        p.Id,
        p.Name,
        p.Price,
        p.OldPrice,
        p.ImageUrl,
        p.CreatedAt
    }));
});

// GET /api/products/popular - Топ товарів (заглушка - рандомні товари)
app.MapGet("/api/products/popular", (IProductsService productsService) =>
{
    var products = productsService.GetAllProducts()
        .OrderBy(_ => Guid.NewGuid())
        .Take(6);
    return Results.Ok(products.Select(p => new
    {
        p.Id,
        p.Name,
        p.Price,
        p.OldPrice,
        p.ImageUrl
    }));
});

// CATEGORIES ENDPOINTS

// GET /api/categories - Отримати всі категорії
app.MapGet("/api/categories", (ICategoriesService categoriesService) =>
{
    var categories = categoriesService.GetAllCategories();
    return Results.Ok(categories.Select(c => new
    {
        c.Id,
        c.Name,
        c.ImageUrl,
        ProductCount = c.Products?.Count ?? 0
    }));
});

// GET /api/categories/{id}
app.MapGet("/api/categories/{id:int}", (int id, ICategoriesService categoriesService) =>
{
    var category = categoriesService.GetCategoryById(id);
    if (category == null) return Results.NotFound();
    return Results.Ok(new
    {
        category.Id,
        category.Name,
        category.ImageUrl,
        Products = category.Products?.Select(p => new { p.Id, p.Name, p.Price, p.ImageUrl })
    });
});

// POST /api/categories - Створити категорію
app.MapPost("/api/categories", (Category category, ICategoriesService categoriesService) =>
{
    categoriesService.AddCategory(category);
    return Results.Created($"/api/categories/{category.Id}", category);
});

// PUT /api/categories/{id} - Оновити категорію
app.MapPut("/api/categories/{id:int}", (int id, Category category, ICategoriesService categoriesService) =>
{
    var existing = categoriesService.GetCategoryById(id);
    if (existing == null) return Results.NotFound();
    
    existing.Name = category.Name;
    existing.ImageUrl = category.ImageUrl;
    categoriesService.UpdateCategory(existing);
    return Results.Ok(existing);
});

// DELETE /api/categories/{id}
app.MapDelete("/api/categories/{id:int}", (int id, ICategoriesService categoriesService) =>
{
    categoriesService.DeleteCategory(id);
    return Results.NoContent();
});

// ============================================
// USERS ENDPOINTS (AUTH)
// ============================================

// POST /api/auth/register - Реєстрація
app.MapPost("/api/auth/register", (User user, IUsersService usersService) =>
{
    var existingUser = usersService.GetUserByEmail(user.Email);
    if (existingUser != null)
        return Results.BadRequest(new { message = "User with this email already exists" });
    
    user.Role = "User"; // Force default role
    usersService.AddUser(user);
    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email, user.Role });
});

// POST /api/auth/login - Авторизація
app.MapPost("/api/auth/login", (LoginRequest request, IUsersService usersService) =>
{
    var user = usersService.GetUserByEmail(request.Email);
    if (user == null || user.PasswordHash != request.Password)
        return Results.Unauthorized();
    
    return Results.Ok(new { user.Id, user.Email, user.Role, Token = $"fake-jwt-token-{user.Id}" });
});

// GET /api/users - Всі користувачі (Admin)
app.MapGet("/api/users", (IUsersService usersService) =>
{
    return Results.Ok(usersService.GetAllUsers().Select(u => new { u.Id, u.Email, u.Role }));
});

// ORDERS ENDPOINTS

// GET /api/orders - Всі замовлення (Admin)
app.MapGet("/api/orders", (IOrdersService ordersService) =>
{
    return Results.Ok(ordersService.GetAllOrders().Select(o => new
    {
        o.Id,
        o.UserId,
        o.CreatedAt,
        o.Status,
        o.TotalAmount,
        ItemsCount = o.Items?.Count ?? 0
    }));
});

// GET /api/orders/{id} - Деталі замовлення
app.MapGet("/api/orders/{id:int}", (int id, IOrdersService ordersService) =>
{
    var order = ordersService.GetOrderById(id);
    if (order == null) return Results.NotFound();
    
    return Results.Ok(new
    {
        order.Id,
        order.UserId,
        order.CreatedAt,
        order.Status,
        order.TotalAmount,
        Items = order.Items?.Select(i => new
        {
            i.ProductId,
            ProductName = i.Product?.Name,
            i.Quantity,
            i.Price
        })
    });
});

// GET /api/orders/my/{userId} - Історія замовлень користувача
app.MapGet("/api/orders/my/{userId:int}", (int userId, IOrdersService ordersService) =>
{
    var orders = ordersService.GetOrdersByUserId(userId);
    return Results.Ok(orders.Select(o => new
    {
        o.Id,
        o.CreatedAt,
        o.Status,
        o.TotalAmount,
        ItemsCount = o.Items?.Count ?? 0
    }));
});

// POST /api/orders - Створити замовлення
app.MapPost("/api/orders", (CreateOrderRequest request, IOrdersService ordersService, IProductsService productsService) =>
{
    var order = new Order
    {
        UserId = request.UserId,
        Status = "New",
        Items = new List<OrderItem>()
    };

    foreach (var item in request.Items)
    {
        var product = productsService.GetProductById(item.ProductId);
        if (product != null)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            });
        }
    }

    ordersService.CreateOrder(order);
    return Results.Created($"/api/orders/{order.Id}", new { order.Id, order.TotalAmount, order.Status });
});

// PUT /api/orders/{id}/status - Оновити статус замовлення (Admin)
app.MapPut("/api/orders/{id:int}/status", (int id, UpdateStatusRequest request, IOrdersService ordersService) =>
{
    var order = ordersService.GetOrderById(id);
    if (order == null) return Results.NotFound();
    
    ordersService.UpdateOrderStatus(id, request.Status);
    return Results.Ok(new { id, Status = request.Status });
});

// ADMIN PRODUCTS ENDPOINTS

// GET /api/admin/products/{id} - Отримати товар для редагування
app.MapGet("/api/admin/products/{id:int}", (int id, IProductsService productsService) =>
{
    var product = productsService.GetProductById(id);
    if (product == null) return Results.NotFound();
    
    return Results.Ok(new
    {
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.OldPrice,
        product.ImageUrl,
        product.StockQuantity,
        product.CategoryId,
        Details = product.Details?.Select(d => new { d.Key, d.Value })
    });
});

// POST /api/admin/products - Створити товар
app.MapPost("/api/admin/products", (Product product, IProductsService productsService) =>
{
    product.CreatedAt = DateTime.UtcNow;
    productsService.AddProduct(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

// PUT /api/admin/products - Редагувати товар
app.MapPut("/api/admin/products", (Product product, IProductsService productsService, ApplicationDbContext context) =>
{
    var existing = productsService.GetProductById(product.Id);
    if (existing == null) return Results.NotFound();
    
    // Update main fields
    existing.Name = product.Name;
    existing.Description = product.Description;
    existing.Price = product.Price;
    existing.OldPrice = product.OldPrice;
    existing.ImageUrl = product.ImageUrl;
    existing.StockQuantity = product.StockQuantity;
    existing.CategoryId = product.CategoryId;
    
    // Update details: remove old, add new
    if (existing.Details != null)
    {
        context.ProductDetails.RemoveRange(existing.Details);
    }
    
    if (product.Details != null)
    {
        existing.Details = product.Details.Select(d => new ProductDetail
        {
            ProductId = product.Id,
            Key = d.Key,
            Value = d.Value
        }).ToList();
    }
    
    productsService.UpdateProduct(existing);
    return Results.Ok(existing);
});

// DELETE /api/admin/products/{id} - Видалити товар
app.MapDelete("/api/admin/products/{id:int}", (int id, IProductsService productsService) =>
{
    productsService.DeleteProduct(id);
    return Results.NoContent();
});

// TEST ENDPOINT

app.MapGet("/", () => "Shop API is running! Endpoints: /api/products, /api/categories, /api/orders, /api/auth/login, /api/auth/register");

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

// REQUEST DTOs

public record LoginRequest(string Email, string Password);
public record CreateOrderRequest(int? UserId, List<OrderItemRequest> Items);
public record OrderItemRequest(int ProductId, int Quantity);
public record UpdateStatusRequest(string Status);