using Inventory.Data;
using Inventory.DTOs;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _user;
        private readonly ILogger<ProductRepository2> _logger;
        private readonly TimeSpan _timeout;

        public AuthRepository(
            AppDbContext context,
            ILogger<ProductRepository2> logger,
            TimeSpan? timeout = null)
        {
            _context = context;
            _user = context.Set<User>();
            _logger = logger;
            _timeout = timeout ?? TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Logs in a user by checking their email and password against the database.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authenticated user if the credentials are valid, null otherwise.</returns>
        public async Task<User?> LogUserAsync(
            User user,
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
                return await _user
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == user.Email, linkedCts.Token);
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Log user was canceled by the caller");

                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Log user timed out");

                throw;
            }
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="newUser">The user to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<bool> AddUserAsync(
            User newUser,
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
                await _user.AddAsync(newUser, linkedCts.Token);
                await _context.SaveChangesAsync(linkedCts.Token);
                return true;
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Add user was canceled by the caller");
                throw;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Add user timed out");
                throw;
            }
        }
    }
}
