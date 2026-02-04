using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly ICategoriesService _categoriesService;

        public ProductsController(IProductsService productsService, ICategoriesService categoriesService)
        {
            _productsService = productsService;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public ActionResult<GetProductsResponse> GetAll()
        {
            var products = _productsService.GetAllProducts();
            return Ok(new GetProductsResponse
            {
                Products = products.Select(p => p.ToDto())
            });
        }

        [HttpGet("{id}")]
        public ActionResult<GetProductByIdResponse> GetById(int id)
        {
            var product = _productsService.GetProductById(id);
            if (product == null) return NotFound();

            return Ok(product.ToDetailDto());
        }

        [HttpGet("filter")]
        public ActionResult<GetProductsResponse> GetByFilter([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? categoryId)
        {
            var products = _productsService.GetProductsByFilter(name, minPrice, maxPrice, categoryId);
            return Ok(new GetProductsResponse
            {
                Products = products.Select(p => p.ToDto())
            });
        }

        [HttpGet("promotional")]
        public ActionResult<GetPromotionalProductsResponse> GetPromotional()
        {
            var products = _productsService.GetPromotionalProducts();
            return Ok(new GetPromotionalProductsResponse
            {
                Products = products.Select(p => p.ToDto())
            });
        }

        [HttpGet("new")]
        public ActionResult<GetNewProductsResponse> GetNew([FromQuery] int count = 10)
        {
            var products = _productsService.GetNewProducts(count);
            return Ok(new GetNewProductsResponse
            {
                Products = products.Select(p => p.ToDto())
            });
        }

        [HttpGet("popular")]
        public ActionResult<GetPopularProductsResponse> GetPopular([FromQuery] int count = 10)
        {
            var products = _productsService.GetPopularProducts(count);
            return Ok(new GetPopularProductsResponse
            {
                Products = products.Select(p => p.ToDto())
            });
        }
    }
}
