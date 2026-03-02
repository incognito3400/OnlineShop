using System.Text.Json;
using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;
using Shop.Mappings;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IImageService _imageService;

        public AdminProductsController(IProductsService productsService, IImageService imageService)
        {
            _productsService = productsService;
            _imageService = imageService;
        }

        [HttpGet("{id}")]
        public ActionResult<GetProductByIdResponse> GetById(int id)
        {
            var product = _productsService.GetProductById(id);
            if (product == null) return NotFound();

            return Ok(product.ToDetailDto());
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductRequest request)
        {
            // Parse Details JSON
            List<ProductDetailDto>? detailsDtos = null;
            if (!string.IsNullOrEmpty(request.Details))
            {
                try 
                {
                    detailsDtos = JsonSerializer.Deserialize<List<ProductDetailDto>>(request.Details, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    // If parsing fails, ignore or return BadRequest. For now, assuming valid JSON or empty.
                }
            }

            // Handle Image (Cloudinary)
            string imageUrl = "/images/default.jpg";
            if (request.Image != null)
            {
                imageUrl = await _imageService.UploadImageAsync(request.Image);
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                OldPrice = request.OldPrice,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            if (detailsDtos != null)
            {
                product.Details = detailsDtos.Select(d => new ProductDetail
                {
                    Key = d.Key,
                    Value = d.Value
                }).ToList();
            }

            _productsService.AddProduct(product);

            // Fetch created product to return full DTO 
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.ToDto());
        }

        [HttpPut]
        public async Task<ActionResult<ProductDto>> Update([FromForm] UpdateProductRequest request)
        {
            var existing = _productsService.GetProductById(request.Id);
            if (existing == null) return NotFound();

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Price = request.Price;
            existing.OldPrice = request.OldPrice;
            existing.StockQuantity = request.StockQuantity;
            existing.CategoryId = request.CategoryId;
            
            if (request.Image != null)
            {
                existing.ImageUrl = await _imageService.UploadImageAsync(request.Image);
            }

            // Update Details
            existing.Details.Clear(); // Only works if loaded into memory (Include in GetById does this)
            if (request.Details != null)
            {
                foreach (var d in request.Details)
                {
                    existing.Details.Add(new ProductDetail
                    {
                        ProductId = existing.Id,
                        Key = d.Key,
                        Value = d.Value
                    });
                }
            }

            _productsService.UpdateProduct(existing);

            return Ok(existing.ToDto());
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var existing = _productsService.GetProductById(id);
            if (existing == null) return NotFound();

            _productsService.DeleteProduct(id);
            return NoContent();
        }
    }
}
