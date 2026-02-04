using Exam2.Backend.Entities;
using Shop.DTOs;

namespace Shop.Mappings
{
    public static class CategoryMapper
    {
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl ?? string.Empty,
                ProductCount = category.Products?.Count ?? 0
            };
        }

        public static CategoryDetailDto ToDetailDto(this Category category)
        {
            return new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl ?? string.Empty,
                ProductCount = category.Products?.Count ?? 0,
                Products = category.Products?.Select(p => p.ToDto()).ToList() ?? new List<ProductDto>()
            };
        }

        public static Category ToEntity(this CreateCategoryRequest request)
        {
            return new Category
            {
                Name = request.Name,
                ImageUrl = request.ImageUrl
            };
        }
    }
}
