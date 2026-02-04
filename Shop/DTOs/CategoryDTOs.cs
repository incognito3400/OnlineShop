using Exam2.Backend.Entities;

namespace Shop.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    public class CategoryDetailDto : CategoryDto
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    public class GetCategoriesResponse
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }

    public class GetCategoryByIdResponse
    {
        public CategoryDetailDto Category { get; set; } = new CategoryDetailDto();
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
