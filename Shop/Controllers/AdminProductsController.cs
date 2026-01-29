using System.Text.Json;
using Exam2.Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Shop.DTOs;
using Shop.Interfaces;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public AdminProductsController(IProductsService productsService)
        {
            _productsService = productsService;
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

        [HttpPost]
        public ActionResult<ProductDto> Create([FromForm] CreateProductRequest request)
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

            // Handle Image (Simulation)
            string imageUrl = "/images/default.jpg";
            if (request.Image != null)
            {
                // In a real app, save file to disk/cloud.
                // For MVP, verify filename or size, but we'll mock the URL.
                imageUrl = $"/images/{request.Image.FileName}"; 
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

            // Fetch created product to return full DTO (though ID is available on product object)
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt
            });
        }

        [HttpPut]
        public ActionResult<ProductDto> Update([FromBody] UpdateProductRequest request)
        {
            var existing = _productsService.GetProductById(request.Id);
            if (existing == null) return NotFound();

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Price = request.Price;
            existing.OldPrice = request.OldPrice;
            existing.StockQuantity = request.StockQuantity;
            existing.CategoryId = request.CategoryId;
            
            if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                existing.ImageUrl = request.ImageUrl;
            }

            // Update Details
            // Naive approach: Clear and Add New (EF Core will handle if configured creating new rows, 
            // but we need to be careful about orphans if we just clear the list in validation.
            // Service handles database update, but we need to modify the entity collection here.
            
            // NOTE: Directly modifying the collection and saving via UpdateProduct might work if cascade delete is on.
            // Better to let EF Core track changes.
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

            return Ok(new ProductDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Price = existing.Price,
                OldPrice = existing.OldPrice,
                ImageUrl = existing.ImageUrl,
                StockQuantity = existing.StockQuantity,
                CategoryId = existing.CategoryId,
                CreatedAt = existing.CreatedAt
            });
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
