using Inventory.DTOs;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    /// <summary>
    /// Represents a controller for handling authentication-related operations, such as user login and token generation.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        /// <summary>
        /// Handles user login requests. Validates the provided email and password, and generates a JWT token if the credentials are valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var token = await _service.LogUserAsync(dto, cancellationToken);

            if (token is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Handles user registration requests. Creates a new user with the provided email, password, and role. Returns a success message if the user is created successfully, or an error message if there was an issue during the creation process.
        /// </summary>
        /// <param name="request">The user registration details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> AddUser(CreateUserDTO request, CancellationToken cancellationToken = default)
        {
            var isAdded = await _service.AddUserAsync(request, cancellationToken);

            if (!isAdded)
            {
                return BadRequest(new
                {
                    message = "there was an error creating the user."
                });
            }

            return Ok(new
            {
                message = "User created successfully."
            });
        }
    }
}
