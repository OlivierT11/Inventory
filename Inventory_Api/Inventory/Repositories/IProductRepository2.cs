using Inventory.Models;

namespace Inventory.Repositories
{
    // Handles database calls
    public interface IProductRepository2
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
