using Inventory.Data;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories
{
    public class ProductRepository2: IProductRepository2
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Product> _products;
        private readonly ILogger<ProductRepository2> _logger;
        private readonly TimeSpan _timeout;

        public ProductRepository2(
            AppDbContext context, 
            ILogger<ProductRepository2> logger,
            TimeSpan? timeout = null)
        {
            _context = context;
            _products = context.Set<Product>();
            _logger = logger;
            _timeout = timeout ?? TimeSpan.FromSeconds(10);
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
                return await _products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id, linkedCts.Token);
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
        /// Gets all products with a timeout of 10 seconds. If the operation takes longer than 10 seconds, it will be canceled.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token (set as default if the method is called outside a controller).</param>
        /// <returns>The list of all products.</returns>
        public async Task<IEnumerable<Product>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                return await _products
                    .AsNoTracking()
                    .ToListAsync(linkedCts.Token);
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Get all products was canceled by the caller");

                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Get all products timed out");

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
                await _products.AddAsync(product);
                await _context.SaveChangesAsync(linkedCts.Token);

                return product;
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
        /// <returns>The updated product or null if not found.</returns>
        public async Task<Product?> UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                var existingProduct = await _products
                    .FirstOrDefaultAsync(p => p.Id == product.Id, linkedCts.Token);

                if (existingProduct is null)
                {
                    return null;
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;

                await _context.SaveChangesAsync(linkedCts.Token);

                return existingProduct;
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
                var product = await _products
                    .FirstOrDefaultAsync(p => p.Id == id, linkedCts.Token);

                if (product is null)
                {
                    return false;
                }

                _products.Remove(product);
                await _context.SaveChangesAsync(linkedCts.Token);

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
