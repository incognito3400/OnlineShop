# Shop Project - Complete Analysis (Verified)

## 1) Project Overview
- **Path**: `D:\Shag\apps\exam3\Shop`
- **Type**: ASP.NET Core Web API
- **Target Framework**: `net10.0`
- **Main goal**: backend API for products, categories, orders, users, auth.

## 2) Technology Stack (Verified)
- ASP.NET Core (`Microsoft.NET.Sdk.Web`)
- Entity Framework Core `10.0.1`
- SQLite (`shop.db`)
- Swagger / OpenAPI (`Swashbuckle.AspNetCore 10.1.0`)
- C# with `nullable` and `implicit usings` enabled.

### NuGet Packages (from `Shop.csproj`)
- `Microsoft.EntityFrameworkCore` 10.0.1
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.1
- `Microsoft.EntityFrameworkCore.SqlServer` 10.0.1
- `Microsoft.EntityFrameworkCore.Tools` 10.0.1
- `Swashbuckle.AspNetCore` 10.1.0

Note: SQL Server provider is installed but current runtime config uses SQLite only.

## 3) Real Source Structure (A-Z)

```text
Shop/
+-- Controllers/
¦   +-- AdminProductsController.cs
¦   +-- AuthController.cs
¦   +-- CategoriesController.cs
¦   +-- OrdersController.cs
¦   +-- ProductsController.cs
¦   L-- UsersController.cs
+-- Database/
¦   +-- ApplicationDbContext.cs
¦   L-- DbInitializer.cs
+-- DTOs/
¦   +-- CategoryDTOs.cs
¦   +-- OrderDTOs.cs
¦   +-- ProductDTOs.cs
¦   L-- UserDTOs.cs
+-- Entities/
¦   +-- Category.cs
¦   +-- Order.cs
¦   +-- OrderItem.cs
¦   +-- Product.cs
¦   +-- ProductDetail.cs
¦   L-- User.cs
+-- Interfaces/
¦   +-- ICategoriesService.cs
¦   +-- IOrdersService.cs
¦   +-- IProductsService.cs
¦   L-- IUsersService.cs
+-- Mappings/
¦   +-- CategoryMapper.cs
¦   +-- OrderMapper.cs
¦   +-- ProductMapper.cs
¦   L-- UserMapper.cs
+-- Services/
¦   +-- CategoriesService.cs
¦   +-- OrdersService.cs
¦   +-- ProductsService.cs
¦   L-- UsersService.cs
+-- Properties/
¦   L-- launchSettings.json
+-- appsettings.Development.json
+-- appsettings.json
+-- Program.cs
+-- Shop.csproj
L-- shop.db
```

Build artifacts (`bin/`, `obj/`) are present but excluded from architecture analysis.

## 4) Architecture (Actual)
Pattern is layered:
- Controllers -> Interfaces -> Services -> `ApplicationDbContext` -> SQLite.
- DTOs + Mappers are used for request/response shaping.
- Entities represent persisted domain data.

### Request flow
1. HTTP request enters controller.
2. Controller maps DTO <-> Entity (via mapping extensions and manual mapping).
3. Service executes business/data logic.
4. EF Core context reads/writes database.
5. Controller returns response DTO.

## 5) Namespaces and Consistency
Important discovery:
- Many files use namespace `Exam2.Backend.*` while project is `Shop`.
- This is technically valid but indicates project rename leftovers.
- `Program.cs` imports `Exam2.Backend.Data` / `Exam2.Backend.Entities`.

## 6) Entities (Verified)
- `User`: `Id`, `Email`, `PasswordHash`, `Role`.
- `Category`: `Id`, `Name`, `ImageUrl`, `Products`.
- `Product`: `Id`, `CategoryId`, `Category`, `Name`, `Description`, `Price`, `OldPrice`, `ImageUrl`, `StockQuantity`, `CreatedAt`, `Details`.
- `ProductDetail`: `Id`, `ProductId`, `Product`, `Key`, `Value`.
- `Order`: `Id`, `UserId`, `CreatedAt`, `Status`, `TotalAmount`, `Items`.
- `OrderItem`: `Id`, `OrderId`, `Order`, `ProductId`, `Product`, `Quantity`, `Price`.

### Relationship model
- Category 1..* Product
- Product 1..* ProductDetail (cascade delete configured)
- Order 1..* OrderItem (cascade delete configured)
- Product 1..* OrderItem
- `Order` has `UserId` but no explicit navigation property to `User`.

## 7) DTOs (Verified)
Your list is mostly correct.
Additions present in code:
- Wrapper responses like `GetProductsResponse`, `GetOrdersResponse`, `GetUsersResponse`, `GetCategoryByIdResponse`, etc.
- `CreateOrderResponse` exists.

## 8) Services (Verified + Notes)
Interfaces and implementations exist for Products/Categories/Orders/Users.

Notable behavior:
- Mostly synchronous EF calls (`SaveChanges`, `ToList`) instead of async.
- `ProductsService.GetPopularProducts` uses `OrderItems` frequency.
- `OrdersService.CreateOrder` calculates `TotalAmount` from items.
- `IUsersService.ValidateCredentials` exists but is not used by `AuthController`.

## 9) Controllers and API Endpoints (Verified)

### `ProductsController` (`/api/products`)
- `GET /api/products`
- `GET /api/products/{id}`
- `GET /api/products/filter`
- `GET /api/products/promotional`
- `GET /api/products/new?count=10`
- `GET /api/products/popular?count=10`

### `AdminProductsController` (`/api/admin/products`)
- `GET /api/admin/products/{id}`
- `POST /api/admin/products` (multipart/form-data, optional image)
- `PUT /api/admin/products`
- `DELETE /api/admin/products/{id}`

### `CategoriesController` (`/api/categories`)
- `GET /api/categories`
- `GET /api/categories/{id}`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}`

### `OrdersController` (`/api/orders`)
- `GET /api/orders`
- `GET /api/orders/{id}`
- `GET /api/orders/my/{userId}`
- `POST /api/orders`
- `PUT /api/orders/{id}/status`

Important: There is **no** DELETE endpoint in OrdersController (despite service having `DeleteOrder`).

### `UsersController` (`/api/users`)
- `GET /api/users`

### `AuthController` (`/api/auth`)
- `POST /api/auth/register`
- `POST /api/auth/login`

### Minimal API endpoints in `Program.cs`
- `GET /` -> status string
- `GET /test-db` -> DB connectivity + row counts

## 10) Database and Initialization
- Provider: SQLite, connection string `Data Source=shop.db`.
- `EnsureCreatedAsync()` called on app startup.
- Seeding is done inside `OnModelCreating` with `HasData` (categories + products).

Important findings:
- `DbInitializer.cs` exists but is not used in `Program.cs`.
- Seeded category/product Ukrainian names appear mojibake (encoding corruption).
- One image URL has corrupted text (`...photo-1461896836934- voices-3...`) and is invalid.

## 11) Configuration
- `appsettings.json` has DB connection + logging + allowed hosts.
- `launchSettings.json`:
  - HTTP: `http://localhost:5295`
  - HTTPS profile includes `https://localhost:7179;http://localhost:5295`
  - launch URL: `swagger`
- Swagger enabled only in Development environment.

## 12) Security Assessment
Current state (critical):
1. Plain-text password storage (`PasswordHash` stores raw password).
2. Fake token generation (`fake-jwt-token-{id}`), no JWT signing/validation.
3. No authentication middleware (`UseAuthentication`, `UseAuthorization` absent).
4. No authorization attributes (`[Authorize]`) on admin/user endpoints.
5. No DTO validation attributes (`[Required]`, `[EmailAddress]`, ranges, etc.).

## 13) Code Quality and Design Risks
- Namespace mismatch (`Exam2.Backend` vs `Shop`) hurts maintainability.
- Dead/unused component: `DbInitializer`.
- `ProductsController` injects `ICategoriesService` but does not use it.
- Nullable contract inconsistency:
  - Interface: `Product? GetProductById(int id)`
  - Service impl returns `Product` with null-forgiving `!`.
- No pagination/sorting on list endpoints.
- No transaction or stock decrement logic on order creation.
- No central exception handling middleware.
- No tests found (unit/integration).

## 14) Correctness Check vs Your Draft

### Accurate in your draft
- Overall layered architecture.
- Main folder layout.
- Entity list and most fields.
- Most endpoints and tech stack.
- Main security weaknesses.

### Corrections needed
1. Orders DELETE endpoint is listed in services, but not exposed by controller.
2. Namespace/domain naming is inconsistent (`Exam2.Backend.*` remains).
3. `DbInitializer.cs` is currently unused.
4. Seed data has encoding corruption and one broken URL.
5. Auth uses direct password comparison and fake token (you noted this correctly, but severity is critical).

## 15) Recommended Priority Plan
1. Implement real auth: password hashing (`BCrypt`/`PBKDF2`), JWT auth, auth middleware.
2. Add authorization for admin routes (`/api/admin/products`, users list, order admin operations).
3. Add request validation attributes and model-state handling.
4. Fix seed encoding and invalid URL; decide one seeding strategy (`HasData` or initializer).
5. Align namespaces to `Shop.*`.
6. Convert services/controllers to async EF methods.
7. Add tests for auth, order creation totals, product filters.

## 16) Final Verdict
Your plan is **~85-90% accurate structurally**.
Main gaps are not structure, but production-readiness and a few real mismatches (Orders DELETE route, namespace leftovers, seed issues, unused initializer).

---
Generated from actual source audit of `D:\Shag\apps\exam3\Shop`.
