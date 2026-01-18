  using Exam2.Backend.Entities;
    using Shop.Interfaces;
    
    using Microsoft.EntityFrameworkCore;
using Exam2.Backend.Data;

namespace Shop.Services
{
  

    public class ProductsService : IProductsService
    {
        private readonly ApplicationDbContext _context;

        public ProductsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Category).Include(p => p.Details).ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Include(p => p.Category).Include(p => p.Details).FirstOrDefault(p => p.Id == id)!;
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Product> GetProductsByFilter(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).Include(p => p.Details).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return query.ToList();
        }
    }
}