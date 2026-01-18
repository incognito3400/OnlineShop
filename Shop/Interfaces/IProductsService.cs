using Exam2.Backend.Entities;

namespace Shop.Interfaces
{
    public interface IProductsService
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);

        IEnumerable<Product> GetProductsByFilter(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId);

    }
}