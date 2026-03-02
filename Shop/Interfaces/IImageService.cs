using Microsoft.AspNetCore.Http;

namespace Shop.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
