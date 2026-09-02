using Inventory.DTOs;
using Inventory.Models;

namespace Inventory.Services
{
    public interface IAuthService
    {
        Task<string?> LogUserAsync(
            LoginDto loginDto,
            CancellationToken cancellationToken = default);

        Task<bool> AddUserAsync(
            CreateUserDTO registerDto,
            CancellationToken cancellationToken = default);
    }
}
