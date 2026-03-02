# Проект Shop - Повний аналіз (Перевірено)

## 1) Огляд проекту
- **Шлях**: `D:\Shag\apps\exam3\Shop`
- **Тип**: ASP.NET Core Web API
- **Цільова платформа (Target Framework)**: `net10.0`
- **Основна мета**: бекенд API для продуктів, категорій, замовлень, користувачів, автентифікації.

## 2) Технологічний стек (Перевірено)
- ASP.NET Core (`Microsoft.NET.Sdk.Web`)
- Entity Framework Core `10.0.1`
- SQLite (`shop.db`)
- Swagger / OpenAPI (`Swashbuckle.AspNetCore 10.1.0`)
- C# з увімкненими `nullable` та `implicit usings`.

### NuGet Пакети (з `Shop.csproj`)
- `CloudinaryDotNet` 1.28.0
- `Microsoft.EntityFrameworkCore` 10.0.1
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.1
- `Microsoft.EntityFrameworkCore.SqlServer` 10.0.1
- `Microsoft.EntityFrameworkCore.Tools` 10.0.1
- `Swashbuckle.AspNetCore` 10.1.0

Примітка: Провайдер SQL Server встановлений, але поточна конфігурація середовища виконання використовує лише SQLite.

## 3) Реальна структура вихідного коду (від А до Я)

```text
Shop/
|- Controllers/
|  |- AdminProductsController.cs
|  |- AuthController.cs
|  |- CategoriesController.cs
|  |- OrdersController.cs
|  |- ProductsController.cs
|  `- UsersController.cs
|- Database/
|  |- ApplicationDbContext.cs
|  `- DbInitializer.cs
|- DTOs/
|  |- CategoryDTOs.cs
|  |- OrderDTOs.cs
|  |- ProductDTOs.cs
|  `- UserDTOs.cs
|- Entities/
|  |- Category.cs
|  |- Order.cs
|  |- OrderItem.cs
|  |- Product.cs
|  |- ProductDetail.cs
|  `- User.cs
|- Interfaces/
|  |- ICategoriesService.cs
|  |- IImageService.cs
|  |- IOrdersService.cs
|  |- IProductsService.cs
|  `- IUsersService.cs
|- Mappings/
|  |- CategoryMapper.cs
|  |- OrderMapper.cs
|  |- ProductMapper.cs
|  `- UserMapper.cs
|- Services/
|  |- CategoriesService.cs
|  |- CloudinaryService.cs
|  |- OrdersService.cs
|  |- ProductsService.cs
|  `- UsersService.cs
|- Properties/
|  `- launchSettings.json
|- appsettings.Development.json
|- appsettings.json
|- Program.cs
|- PROJECT_ANALYSIS.md
|- Shop.csproj
`- shop.db
```

Артефакти збірки (`bin/`, `obj/`) присутні, але виключені з аналізу архітектури.

## 4) Архітектура (Фактична)
Шаблон шаруватий:
- Контролери -> Інтерфейси -> Сервіси -> `ApplicationDbContext` -> SQLite.
- DTO + Mappers використовуються для формування запитів/відповідей.
- Сутності (Entities) представляють збережені доменні дані.

### Потік виконання запиту
1. HTTP запит надходить у контролер.
2. Контролер перетворює DTO <-> Entity (через методи розширення або вручну).
3. Сервіс виконує бізнес/дані логіку.
4. Контекст EF Core читає/записує в базу даних.
5. Контролер повертає DTO у відповідь.

## 5) Простори імен та Узгодженість
Важливе відкриття:
- Багато файлів використовують простір імен `Exam2.Backend.*`, хоча проект називається `Shop`.
- Технічно це можливо, але вказує на залишки від перейменування проекту.
- `Program.cs` імпортує `Exam2.Backend.Data` / `Exam2.Backend.Entities`.

## 6) Сутності (Перевірено)
- `User`: `Id`, `Email`, `PasswordHash`, `Role`, `ImageUrl`.
- `Category`: `Id`, `Name`, `ImageUrl`, `Products`.
- `Product`: `Id`, `CategoryId`, `Category`, `Name`, `Description`, `Price`, `OldPrice`, `ImageUrl`, `StockQuantity`, `CreatedAt`, `Details`.
- `ProductDetail`: `Id`, `ProductId`, `Product`, `Key`, `Value`.
- `Order`: `Id`, `UserId`, `CreatedAt`, `Status`, `TotalAmount`, `Items`.
- `OrderItem`: `Id`, `OrderId`, `Order`, `ProductId`, `Product`, `Quantity`, `Price`.

### Модель зв'язків
- Category 1..* Product
- Product 1..* ProductDetail (налаштовано каскадне видалення)
- Order 1..* OrderItem (налаштовано каскадне видалення)
- Product 1..* OrderItem
- `Order` має `UserId`, але не має явної навігаційної властивості `User`.

## 7) DTOs (Перевірено)
Ваш список здебільшого правильний.
Додатки, присутні в коді:
- Відповіді-обгортки, такі як `GetProductsResponse`, `GetOrdersResponse`, `GetUsersResponse`, `GetCategoryByIdResponse`, тощо.
- Присутній `CreateOrderResponse`.
- Додано `IFormFile Image` до `CreateProductRequest`, `UpdateProductRequest`, `RegisterRequest` для підтримки завантаження зображень через Cloudinary.
- Додано `ImageUrl` до `UserDto` та `LoginResponse`.

## 8) Сервіси (Перевірено + Примітки)
Існують інтерфейси та реалізації для Продуктів, Категорій, Замовлень, Користувачів.

Помітна поведінка:
- Переважно використовуються синхронні виклики EF (`SaveChanges`, `ToList`) замість асинхронних.
- `ProductsService.GetPopularProducts` використовує частоту `OrderItems`.
- `OrdersService.CreateOrder` обчислює `TotalAmount` з елементів (items).
- `IUsersService.ValidateCredentials` існує, але не використовується в `AuthController`.

## 9) Контролери та API Кінцеві точки (Перевірено)

### `ProductsController` (`/api/products`)
- `GET /api/products?categoryId=...&search=...&sort=...`
- `GET /api/products/{id}`
- `GET /api/products/filter`
- `GET /api/products/promotions`
- `GET /api/products/promotional` (застарілий псевдонім)
- `GET /api/products/new?count=10`
- `GET /api/products/popular?count=10`

### `AdminProductsController` (`/api/admin/products`)
- `GET /api/admin/products/{id}`
- `POST /api/admin/products` (multipart/form-data, необов'язкове зображення)
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

Важливо: В OrdersController **немає** кінцевої точки DELETE (хоча в сервісі є `DeleteOrder`).

### `UsersController` (`/api/users`)
- `GET /api/users`

### `AuthController` (`/api/auth`)
- `POST /api/auth/register` (multipart/form-data, необов'язкове зображення)
- `POST /api/auth/login`

### Мінімальні API кінцеві точки в `Program.cs`
- `GET /` -> статус-рядок
- `GET /test-db` -> підключення до БД + підрахунок рядків

## 10) База даних та Ініціалізація
- Провайдер: SQLite, рядок підключення `Data Source=shop.db`.
- `EnsureCreatedAsync()` викликається при запуску додатку.
- Посів (Seeding) здійснюється всередині `OnModelCreating` через `HasData` (категорії + продукти).

Важливі знахідки:
- `DbInitializer.cs` існує, але не використовується в `Program.cs`.
- Засіяні українські назви категорій/продуктів мають кракозябри (пошкоджене кодування).
- Одна URL-адреса зображення містить пошкоджений текст (`...photo-1461896836934- voices-3...`) і є недійсною.

## 11) Конфігурація
- `appsettings.json` містить рядок підключення до БД + логування + дозволені хости.
- `appsettings.Development.json` містить налаштування `CloudinarySettings` (CloudName, ApiKey, ApiSecret).
- `launchSettings.json`:
  - HTTP: `http://localhost:5295`
  - Профіль HTTPS: `https://localhost:7179;http://localhost:5295`
  - стартовий URL: `swagger`
- Swagger увімкнено лише в середовищі Development.

## 12) Оцінка безпеки
Поточний стан (критичний):
1. Зберігання паролів у відкритому вигляді (`PasswordHash` зберігає необроблений пароль).
2. Фейкова генерація токена (`fake-jwt-token-{id}`), немає підпису/валідації JWT.
3. Відсутнє проміжне програмне забезпечення автентифікації (`UseAuthentication`, `UseAuthorization` відсутні).
4. Відсутні атрибути авторизації (`[Authorize]`) на кінцевих точках адміністратора/користувача.
5. Відсутні атрибути валідації DTO (`[Required]`, `[EmailAddress]`, діапазони, тощо).

## 13) Якість коду та Ризики дизайну
- Невідповідність просторів імен (`Exam2.Backend` vs `Shop`) ускладнює підтримку.
- Мертвий/невикористовуваний компонент: `DbInitializer`.
- Неузгодженість контрактів nullable:
  - Інтерфейс: `Product? GetProductById(int id)`
  - Реалізація сервісу повертає `Product` з null-forgiving оператором `!`.
- Відсутня пагінація на кінцевих точках списків; сортування часткове (`priceasc`, `pricedesc`, `new`) та не задокументоване як enum/контракт.
- Немає транзакцій або логіки зменшення залишку товару (StockQuantity) при створенні замовлення.
- Відсутня централізована обробка виключень (middleware).
- Тести не знайдено (unit/integration).

## 14) Перевірка правильності вашої чернетки

### Правильно у вашій чернетці
- Загальна шарувата архітектура.
- Структура основних папок.
- Список сутностей та більшість полів.
- Більшість кінцевих точок та технологічний стек.
- Основні проблеми безпеки.

### Необхідні виправлення
1. Кінцеву точку DELETE для замовлень вказано в сервісах, але її немає в контролері.
2. Неузгодженість назв простору імен/домену (залишилось `Exam2.Backend.*`).
3. `DbInitializer.cs` наразі не використовується.
4. Дані посіву мають пошкоджене кодування та одну зламану URL-адресу.
5. Авторизація використовує пряме порівняння паролів і фейковий токен (ви правильно відзначили це, але ступінь критичності високий).

## 15) Рекомендований пріоритетний план
1. Реалізувати справжню автентифікацію: хешування паролів (`BCrypt`/`PBKDF2`), JWT автентифікацію, middleware.
2. Додати авторизацію для адмін-маршрутів (`/api/admin/products`, список користувачів, операції адміністратора з замовленнями).
3. Додати атрибути валідації запитів (Request validation) та обробку стану моделі (model-state).
4. Виправити кодування посіву та недійсну URL-адресу; визначитися з однією стратегією посіву (`HasData` або ініціалізатор).
5. Привести простори імен у відповідність до `Shop.*`.
6. Перевести сервіси/контролери на асинхронні методи EF.
7. Додати тести для автентифікації, загальної суми при створенні замовлення, фільтрів продуктів.
