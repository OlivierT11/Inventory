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
            CancellationToken cancellationToken = default)
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

        public async Task<ProductListDto> GetWithPager(
            int page,
            CancellationToken cancellationToken = default)
        {
            const int pageSize = 10;

            // Compter le nombre total d'éléments dans la table Products
            var query = _context.Products
                .AsNoTracking()
                .Select(product => new ProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock 

                });
            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Récupérer les produits pour la page demandée
            var products = await query
                .Skip((page - 1) * pageSize)  // Skip(20) ignore les 20 premiers produits.
                .Take(pageSize) // Take(10) récupère au maximum 10 produits.
                .ToListAsync(cancellationToken);

            // Construire le DTO de réponse avec les informations de pagination
            var productListDto = new ProductListDto
            {
                Products = products,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return productListDto;
        }

        public async Task<ProductResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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
