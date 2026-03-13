using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public ActionResult<GetCategoriesResponse> GetAll()
        {
            var categories = _categoriesService.GetAllCategories();
            return Ok(new GetCategoriesResponse
            {
                Categories = categories.Select(c => c.ToDto())
            });
        }

        [HttpGet("{id}")]
        public ActionResult<GetCategoryByIdResponse> GetById(int id)
        {
            var category = _categoriesService.GetCategoryById(id);
            if (category == null) return NotFound();

            return Ok(new GetCategoryByIdResponse
            {
                Category = category.ToDetailDto()
            });
        }

        [HttpPost]
        public ActionResult<CategoryDto> Create([FromBody] CreateCategoryRequest request)
        {
            var category = request.ToEntity();
            _categoriesService.AddCategory(category);
            
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category.ToDto());
        }

        [HttpPut("{id}")]
        public ActionResult<CategoryDto> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            var existing = _categoriesService.GetCategoryById(id);
            if (existing == null) return NotFound();

            existing.Name = request.Name;
            existing.ImageUrl = request.ImageUrl;
            
            _categoriesService.UpdateCategory(existing);
            return Ok(existing.ToDto());
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
             var existing = _categoriesService.GetCategoryById(id);
             if (existing == null) return NotFound();

            _categoriesService.DeleteCategory(id);
            return NoContent();
        }
    }
}
