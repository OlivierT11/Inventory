using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Services
{
    /// <summary>
    /// Represents a service for creating JWT tokens.
    /// </summary>
    public sealed class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="role">The user's role.</param>
        /// <returns>The JWT token.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JWT key is missing.</exception>
        public string CreateToken(int userId, string email, string role)
        {
            var jwt = _configuration.GetSection("Jwt");
            var key = jwt["Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwt["ExpiresMinutes"] ?? "60")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
