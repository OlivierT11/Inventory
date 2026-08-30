using Inventory.Data;
using Inventory.DTOs;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Select(product => new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            string cacheKey = $"product:{id}";

            if (_cache.TryGetValue<ProductResponseDto>(cacheKey, out var cachedProduct))
            {
                Console.WriteLine("Value used from cache.");
                return cachedProduct;
            }

            var product = await _context.Products
                .AsNoTracking()
                .Where(product => product.Id == id)
                .Select(product => new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product != null)
            {
                _cache.Set(
                    cacheKey,
                    product,
                    TimeSpan.FromMinutes(5));
            }

            Console.WriteLine("Value used from DB.");
            return product;
        }

        public async Task<ProductResponseDto> CreateAsync(
            ProductCreateDto dto,
            CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };
        }

        public async Task<bool> UpdateAsync(
            int id,
            ProductUpdateDto dto,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product is null)
            {
                return false;
            }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product is null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
