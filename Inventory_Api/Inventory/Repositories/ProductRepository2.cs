using Inventory.Data;
using Inventory.DTOs;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory.Repositories
{
    public class ProductRepository2 : IProductRepository2
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Product> _products;
        private readonly ILogger<ProductRepository2> _logger;
        private readonly TimeSpan _timeout;
        private readonly IMemoryCache _cache;


        public ProductRepository2(
            AppDbContext context,
            ILogger<ProductRepository2> logger,
            IMemoryCache cache,
            TimeSpan? timeout = null)
        {
            _context = context;
            _products = context.Set<Product>();
            _logger = logger;
            _timeout = timeout ?? TimeSpan.FromSeconds(10);
            _cache = cache;
        }

        public async Task<List<Product>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            // timeout token
            using var timeoutCts = new CancellationTokenSource(_timeout);

            // link the cancellation token with the timeout token
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                return await _context.Products
                .AsNoTracking()
                .Select(product => new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                })
                .ToListAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Get product was canceled by the caller");

                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Get product timed out");

                throw;
            }
        }

        public async Task<ProductListDto> GetWithPager(
            int page,
            CancellationToken cancellationToken = default)
        {
            // timeout token
            using var timeoutCts = new CancellationTokenSource(_timeout);

            // link the cancellation token with the timeout token
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            const int pageSize = 10;

            try
            {
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
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Get product was canceled by the caller");

                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Get product timed out");

                throw;
            }
        }

        /// <summary>
        /// Gets a product by its ID with a timeout of 10 seconds. If the operation takes longer than 10 seconds, it will be canceled.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <param name="cancellationToken">The cancellation token (set as default if the method is called outside a controller).</param>
        /// <returns>The requested product or null if not found.</returns>
        public async Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            // timeout token
            using var timeoutCts = new CancellationTokenSource(_timeout);

            // link the cancellation token with the timeout token
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                string cacheKey = $"product:{id}";

                if (_cache.TryGetValue<Product>(cacheKey, out var cachedProduct))
                {
                    Console.WriteLine("Value used from cache.");
                    return cachedProduct;
                }

                var product = await _context.Products
                    .AsNoTracking()
                    .Where(product => product.Id == id)
                    .Select(product => new Product
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
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Get product was canceled by the caller");

                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Get product timed out");

                throw;
            }
        }

        /// <summary>
        /// Creates a new product with a timeout of 10 seconds. If the operation takes longer than 10 seconds, it will be canceled.
        /// </summary>
        /// <param name="product">The product to create.</param>
        /// <param name="cancellationToken">The cancellation token (set as default if the method is called outside a controller).</param>
        /// <returns>The created product.</returns>
        public async Task<Product> CreateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync(cancellationToken);

                return new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                };
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Create product was canceled by the caller");
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Create product timed out");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing product with a timeout of 10 seconds. If the operation takes longer than 10 seconds, it will be canceled.
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <param name="cancellationToken">The cancellation token (set as default if the method is called outside a controller).</param>
        /// <returns>A boolean indicating whether the product was updated.</returns>
        public async Task<bool> UpdateAsync(
            int id,
            Product product,
            CancellationToken cancellationToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                var updatedProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (updatedProduct is null)
                {
                    return false;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Update product was canceled by the caller");
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Update product timed out");
                throw;
            }
        }

        /// <summary>
        /// Deletes a product by its ID with a timeout of 10 seconds. If the operation takes longer than 10 seconds, it will be canceled.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <param name="cancellationToken">The cancellation token (set as default if the method is called outside a controller).</param>
        /// <returns>A boolean indicating whether the product was deleted.</returns>
        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
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
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Delete product was canceled by the caller");
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Delete product timed out");
                throw;
            }
        }
    }
}
