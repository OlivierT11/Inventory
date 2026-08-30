using Inventory.DTOs;

namespace Inventory.Services
{
    // Handles business logic. Calls the repository
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<ProductResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken);

        Task<ProductResponseDto> CreateAsync(
            ProductCreateDto dto,
            CancellationToken cancellationToken);

        Task<bool> UpdateAsync(
            int id,
            ProductUpdateDto dto,
            CancellationToken cancellationToken);

        Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken);
    }
}
