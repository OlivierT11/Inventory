using Inventory.DTOs;
using Inventory.Models;

namespace Inventory.Services
{
    // Handles business logic. Calls the repository
    public interface IProductService2
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
