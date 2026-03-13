using Microsoft.EntityFrameworkCore;
using Exam2.Backend.Data;
using Exam2.Backend.Entities;
using Shop.Interfaces;
using Shop.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "SuperSecretKeyForJwtAuthenticationInThisApp");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//  Cloudinary Settings
builder.Services.Configure<Shop.Configuration.CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IImageService, CloudinaryService>();


var app = builder.Build();


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

    var dbConnection = context.Database.GetDbConnection();
    await dbConnection.OpenAsync();

    var executeQuietly = async (string sql) =>
    {
        try
        {
            using var command = dbConnection.CreateCommand();
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }
        catch { }
    };

    await executeQuietly("ALTER TABLE Users ADD COLUMN ImageUrl TEXT NULL;");
    await executeQuietly("ALTER TABLE Users ADD COLUMN RefreshToken TEXT NULL;");
    await executeQuietly("ALTER TABLE Users ADD COLUMN RefreshTokenExpiryTime TEXT NULL;");

    await dbConnection.CloseAsync();
}

app.UseAuthentication();
app.UseAuthorization();

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