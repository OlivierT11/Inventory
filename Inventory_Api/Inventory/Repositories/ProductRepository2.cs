using Inventory.Data;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories
{
    public class ProductRepository2
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Product> _products;

        public ProductRepository2(AppDbContext context)
        {
            _context = context;
            _products = context.Set<Product>();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            var existingProduct = await _products
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct is null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;

            await _context.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return false;
            }

            _products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
