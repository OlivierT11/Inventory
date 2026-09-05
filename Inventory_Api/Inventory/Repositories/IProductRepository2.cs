using Inventory.DTOs;
using Inventory.Models;

namespace Inventory.Repositories
{
    // Handles database calls
    public interface IProductRepository2
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductListDto> GetWithPager(int page, CancellationToken cancellationToken = default);
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
