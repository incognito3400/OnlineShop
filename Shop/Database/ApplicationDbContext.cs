using Exam2.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exam2.Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductDetail> ProductDetails { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Product -> Details relation
        modelBuilder.Entity<ProductDetail>()
            .HasOne(pd => pd.Product)
            .WithMany(p => p.Details)
            .HasForeignKey(pd => pd.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Order -> OrderItem relation
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Електроніка", ImageUrl = "https://images.unsplash.com/photo-1498049794561-7780e7231661?w=400" },
            new Category { Id = 2, Name = "Одяг", ImageUrl = "https://images.unsplash.com/photo-1445205170230-053b83016050?w=400" },
            new Category { Id = 3, Name = "Дім та сад", ImageUrl = "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=400" },
            new Category { Id = 4, Name = "Спорт", ImageUrl = "https://images.unsplash.com/photo-1461896836934- voices-3?w=400" },
            new Category { Id = 5, Name = "Книги", ImageUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=400" }
        );

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Email = "admin@example.com", PasswordHash = "admin123", Role = "Admin", ImageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400" },
            new User { Id = 2, Email = "test@example.com", PasswordHash = "test123", Role = "User", ImageUrl = "https://images.unsplash.com/photo-1527980965255-d3b416303d12?w=400" },
            new User { Id = 3, Email = "client@example.com", PasswordHash = "client123", Role = "User", ImageUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, CategoryId = 1, Name = "Смартфон Samsung Galaxy A54", Description = "6.4\" Super AMOLED, 128GB, 5G", Price = 13999m, OldPrice = 15999m, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400", StockQuantity = 50, CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, CategoryId = 1, Name = "Ноутбук ASUS VivoBook 15", Description = "15.6\" FHD, Intel i5, 16GB RAM, 512GB SSD", Price = 25999m, OldPrice = null, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400", StockQuantity = 25, CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 3, CategoryId = 1, Name = "Навушники Sony WH-1000XM5", Description = "Бездротові, шумопоглинання", Price = 11499m, OldPrice = 13999m, ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400", StockQuantity = 100, CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 4, CategoryId = 1, Name = "Планшет Apple iPad Air", Description = "10.9\", 64GB, Wi-Fi", Price = 24999m, OldPrice = 27999m, ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400", StockQuantity = 30, CreatedAt = new DateTime(2024, 4, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 5, CategoryId = 2, Name = "Куртка зимова чоловіча", Description = "Водонепроникна, розміри M-XXL", Price = 3499m, OldPrice = 4299m, ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400", StockQuantity = 75, CreatedAt = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 6, CategoryId = 2, Name = "Кросівки Nike Air Max", Description = "Унісекс, всі розміри", Price = 4999m, OldPrice = null, ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", StockQuantity = 150, CreatedAt = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 7, CategoryId = 2, Name = "Джинси класичні", Description = "Бавовна 100%, сині", Price = 1299m, OldPrice = 1599m, ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400", StockQuantity = 100, CreatedAt = new DateTime(2024, 4, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 8, CategoryId = 3, Name = "Набір садових інструментів", Description = "12 предметів, сталь", Price = 1299m, OldPrice = 1599m, ImageUrl = "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=400", StockQuantity = 40, CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 9, CategoryId = 3, Name = "LED лампа настільна", Description = "10W, димер, USB зарядка", Price = 799m, OldPrice = null, ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=400", StockQuantity = 200, CreatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 10, CategoryId = 3, Name = "Кавоварка еспресо", Description = "Ріжкова, 15 бар", Price = 5499m, OldPrice = 6999m, ImageUrl = "https://images.unsplash.com/photo-1517246225969-2f1ebbc5dc65?w=400", StockQuantity = 20, CreatedAt = new DateTime(2024, 4, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 11, CategoryId = 4, Name = "Гантелі 2x10 кг", Description = "Гумове покриття, хромована ручка", Price = 1899m, OldPrice = 2199m, ImageUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400", StockQuantity = 60, CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 12, CategoryId = 4, Name = "Yoga мат професійний", Description = "183x61см, 6мм товщина", Price = 699m, OldPrice = null, ImageUrl = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=400", StockQuantity = 120, CreatedAt = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 13, CategoryId = 4, Name = "Фітнес-браслет Xiaomi", Description = "Pulse Ox, AMOLED", Price = 1599m, OldPrice = 1899m, ImageUrl = "https://images.unsplash.com/photo-1575311373937-040b8e1fd5b0?w=400", StockQuantity = 200, CreatedAt = new DateTime(2024, 4, 18, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 14, CategoryId = 5, Name = "Кобзар - Тарас Шевченко", Description = "Подарункове видання, тверда обкладинка", Price = 450m, OldPrice = 550m, ImageUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=400", StockQuantity = 80, CreatedAt = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 15, CategoryId = 5, Name = "Мистецтво війни - Сунь Цзи", Description = "Переклад українською, м'яка обкладинка", Price = 250m, OldPrice = 300m, ImageUrl = "https://images.unsplash.com/photo-1589829085413-56de8ae18c73?w=400", StockQuantity = 300, CreatedAt = new DateTime(2024, 4, 20, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
