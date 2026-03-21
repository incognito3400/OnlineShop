using Microsoft.AspNetCore.Mvc;
using Shop.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Shop.Configuration;

namespace Shop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BulkUploadController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _localImagesPath;

        public BulkUploadController(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
            
            // Default path where user should put images
            _localImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp_images");
        }

        [HttpPost("upload-products")]
        public async Task<IActionResult> UploadProductImages()
        {
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "photo");
            if (!Directory.Exists(imagesPath))
            {
                return NotFound($"Directory not found: {imagesPath}. Please ensure your photos are in Shop/Database/photo");
            }

            // Support common image formats
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".webp", ".avif" };
            var files = Directory.GetFiles(imagesPath)
                                 .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                                 .ToList();
            
            var results = new List<object>();

            foreach (var file in files)
            {
                var originalName = Path.GetFileNameWithoutExtension(file);
                
                // Extract only the digits from the filename (e.g., "10 (2)" becomes "10")
                var match = System.Text.RegularExpressions.Regex.Match(originalName, @"\d+");
                if (!match.Success) continue;

                var cleanId = match.Value;
                var publicId = $"shop/products/{cleanId}";

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file),
                    PublicId = publicId,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                results.Add(new { 
                    OriginalName = originalName, 
                    PublicId = publicId, 
                    Status = uploadResult.StatusCode.ToString() 
                });
            }

            return Ok(new { Message = $"Processed {results.Count} images", Details = results });
        }

        [HttpPost("upload-categories")]
        public async Task<IActionResult> UploadCategoryImages()
        {
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "photo", "categories");
            if (!Directory.Exists(imagesPath))
            {
                return NotFound($"Directory not found: {imagesPath}. Please create Shop/Database/photo/categories");
            }

            var files = Directory.GetFiles(imagesPath, "*.jpg");
            var results = new List<object>();

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var publicId = $"shop/categories/{fileName}";

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file),
                    PublicId = publicId,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                results.Add(new { FileName = fileName, Status = uploadResult.StatusCode.ToString(), Url = uploadResult.SecureUrl });
            }

            return Ok(results);
        }
    }
}
