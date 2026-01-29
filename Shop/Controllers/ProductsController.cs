using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("promotions")]
        public ActionResult<IEnumerable<ProductDto>> GetPromotions()
        {
            var products = _productsService.GetPromotionalProducts();
            return Ok(products.Select(MapToDto));
        }

        [HttpGet("new")]
        public ActionResult<IEnumerable<ProductDto>> GetNewProducts()
        {
            var products = _productsService.GetNewProducts(10); // Default 10
            return Ok(products.Select(MapToDto));
        }

        [HttpGet("popular")]
        public ActionResult<IEnumerable<ProductDto>> GetPopularProducts()
        {
            var products = _productsService.GetPopularProducts(6); // Default 6
            return Ok(products.Select(MapToDto));
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetAll([FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var products = _productsService.GetProductsByFilter(search, minPrice, maxPrice, categoryId);
            return Ok(products.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public ActionResult<GetProductByIdResponse> GetById(int id)
        {
            var product = _productsService.GetProductById(id);
            if (product == null) return NotFound();

            return Ok(new GetProductByIdResponse
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
                Details = product.Details.Select(d => new ProductDetailDto { Key = d.Key, Value = d.Value }).ToList()
            });
        }

        private static ProductDto MapToDto(Exam2.Backend.Entities.Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                OldPrice = p.OldPrice,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
