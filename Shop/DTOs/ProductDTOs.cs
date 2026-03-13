using Microsoft.AspNetCore.Http;

namespace Shop.DTOs
{
    // Public Responses
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductDetailDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class GetProductByIdResponse : ProductDto
    {
        public string Description { get; set; } = string.Empty;
        public List<ProductDetailDto> Details { get; set; } = new();
    }

    public class GetProductsResponse
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    public class GetNewProductsResponse
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    public class GetPopularProductsResponse
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    public class GetPromotionalProductsResponse
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    // Admin Requests
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public string Details { get; set; } = "[]"; // JSON String
    }

    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductDetailDto> Details { get; set; } = new();
    }
}
