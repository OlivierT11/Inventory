using Inventory.DTOs;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
        private readonly TokenRevocationService _tokenRevocationService;

        public AuthController(IAuthService service, TokenRevocationService tokenRevocationService)
        {
            _service = service;
            _tokenRevocationService = tokenRevocationService;
        }

        /// <summary>
        /// Handles user login requests. Validates the provided email and password, and generates a JWT token if the credentials are valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")] //(url)/api/auth/login
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var token = await _service.LogUserAsync(dto, cancellationToken);

            if (token is null)
            {
                return Unauthorized(new  // Unauthorized() is part of IActionResult, as well as OK()
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(new { access_token = token });
        }

        /// <summary>
        /// Handles user logout requests. Revokes the JWT token provided in the Authorization header, preventing further use of that token for authentication.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Extract the JWT token from the Authorization header
            var token = Request.Headers.Authorization
                .ToString()
                .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Missing access token.");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Extract the jti (JWT ID) claim from the token, which is used to uniquely identify the token for revocation purposes.
            var jti = jwtToken.Claims
                .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)
            ?.Value;

            if (string.IsNullOrWhiteSpace(jti))
            {
                return BadRequest("Token does not contain a jti claim.");
            }

            _tokenRevocationService.Revoke(
                jti,
                jwtToken.ValidTo
            );

            return NoContent();
        }


        /// <summary>
        /// Handles user registration requests. Creates a new user with the provided email, password, and role. Returns a success message if the user is created successfully, or an error message if there was an issue during the creation process.
        /// </summary>
        /// <param name="request">The user registration details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("register")] //(url)/api/auth/register
        public async Task<IActionResult> AddUser(CreateUserDTO request, CancellationToken cancellationToken = default)
        {
            var isAdded = await _service.AddUserAsync(request, cancellationToken);

            if (!isAdded)
            {
                return BadRequest(new
                {
                    message = "This user already exists."
                });
            }

            return Ok(new
            {
                message = "User created successfully."
            });
        }
    }
}
