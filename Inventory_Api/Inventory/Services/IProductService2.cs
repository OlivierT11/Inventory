using Inventory.DTOs;
using Inventory.Models;

namespace Inventory.Services
{
    // Handles business logic. Calls the repository
    public interface IProductService2
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
