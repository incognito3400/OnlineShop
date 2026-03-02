using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shop.Migrations
{
    /// <inheritdoc />
    public partial class AddUserImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://images.unsplash.com/photo-1498049794561-7780e7231661?w=400", "Електроніка" },
                    { 2, "https://images.unsplash.com/photo-1445205170230-053b83016050?w=400", "Одяг" },
                    { 3, "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=400", "Дім та сад" },
                    { 4, "https://images.unsplash.com/photo-1461896836934- voices-3?w=400", "Спорт" },
                    { 5, "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=400", "Книги" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "Name", "OldPrice", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "6.4\" Super AMOLED, 128GB, 5G", "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400", "Смартфон Samsung Galaxy A54", 15999m, 13999m, 50 },
                    { 2, 1, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "15.6\" FHD, Intel i5, 16GB RAM, 512GB SSD", "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400", "Ноутбук ASUS VivoBook 15", null, 25999m, 25 },
                    { 3, 1, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Бездротові, шумопоглинання", "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400", "Навушники Sony WH-1000XM5", 13999m, 11499m, 100 },
                    { 4, 2, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Водонепроникна, розміри M-XXL", "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400", "Куртка зимова чоловіча", 4299m, 3499m, 75 },
                    { 5, 2, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Унісекс, всі розміри", "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", "Кросівки Nike Air Max", null, 4999m, 150 },
                    { 6, 3, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "12 предметів, сталь", "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=400", "Набір садових інструментів", 1599m, 1299m, 40 },
                    { 7, 3, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "10W, димер, USB зарядка", "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=400", "LED лампа настільна", null, 799m, 200 },
                    { 8, 4, new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Гумове покриття, хромована ручка", "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400", "Гантелі 2x10 кг", 2199m, 1899m, 60 },
                    { 9, 4, new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "183x61см, 6мм товщина", "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=400", "Yoga мат професійний", null, 699m, 120 },
                    { 10, 5, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Подарункове видання, тверда обкладинка", "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=400", "Кобзар - Тарас Шевченко", 550m, 450m, 80 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductDetails");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
