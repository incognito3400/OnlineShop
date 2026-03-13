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
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
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

        public IEnumerable<Product> GetPromotionalProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.OldPrice != null && p.OldPrice > p.Price)
                .ToList();
        }

        public IEnumerable<Product> GetNewProducts(int count)
        {
            return _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToList();
        }

        public IEnumerable<Product> GetPopularProducts(int count)
        {
           
           
             var popularIds = _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToList();

            if (!popularIds.Any())
            {
                return _context.Products.Include(p => p.Category).Take(count).ToList();
            }

            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => popularIds.Contains(p.Id))
                .ToList();
            
            // Restore order
            return popularIds.Select(id => products.FirstOrDefault(p => p.Id == id)).Where(p => p != null).ToList()!;
        }
    }
}