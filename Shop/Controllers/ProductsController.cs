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

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        public ActionResult<GetProductsResponse> GetAll(
            [FromQuery] int? categoryId,
            [FromQuery] string? search,
            [FromQuery] string? sort)
        {
            var products = _productsService.GetProductsByFilter(search, null, null, categoryId);

            if (!string.IsNullOrWhiteSpace(sort))
            {
                products = sort.ToLowerInvariant() switch
                {
                    "priceasc" => products.OrderBy(p => p.Price),
                    "pricedesc" => products.OrderByDescending(p => p.Price),
                    "new" => products.OrderByDescending(p => p.CreatedAt),
                    _ => products
                };
            }

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

        [HttpGet("promotions")]
        public ActionResult<GetPromotionalProductsResponse> GetPromotions()
        {
            var products = _productsService.GetPromotionalProducts();
            return Ok(new GetPromotionalProductsResponse
            {
                Products = products.Select(p => p.ToDto())
            });
        }

        [HttpGet("promotional")]
        public ActionResult<GetPromotionalProductsResponse> GetPromotionalLegacy()
        {
            return GetPromotions();
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
