using Inventory.DTOs;
using Inventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Inventory.Controllers
{
    /// <summary>
    /// Represents a controller for managing user profiles.
    /// </summary>
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {

        /// <summary>
        /// Gets the profile information of the authenticated user.
        /// </summary>
        /// <returns></returns>
        [Authorize] // uses app.UseAuthorization();
        [HttpGet]
        public IActionResult GetProfile()
        {
            return Ok(new
            {
                userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
                email = User.FindFirstValue(JwtRegisteredClaimNames.Email),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        /// <summary>
        /// Deletes the profile of the user with the specified ID. This action is restricted to users with the "Admin" role.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteProfile(int id)
        {
            return NoContent();
        }
    }
}
