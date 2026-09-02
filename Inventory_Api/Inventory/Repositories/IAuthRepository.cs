using Inventory.DTOs;
using Inventory.Models;

namespace Inventory.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> LogUserAsync(
            User user,
            CancellationToken cancellationToken = default);

        Task<bool> AddUserAsync(
            User newUser,
            CancellationToken cancellationToken = default);
    }

}
