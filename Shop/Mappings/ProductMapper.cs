using Exam2.Backend.Entities;
using Shop.DTOs;

namespace Shop.Mappings
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                CreatedAt = product.CreatedAt
            };
        }

        public static GetProductByIdResponse ToDetailDto(this Product product)
        {
            return new GetProductByIdResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                CreatedAt = product.CreatedAt,
                Description = product.Description,
                Details = product.Details.Select(d => new ProductDetailDto 
                { 
                    Key = d.Key, 
                    Value = d.Value 
                }).ToList()
            };
        }
    }
}
