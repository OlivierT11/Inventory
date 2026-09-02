using System.ComponentModel.DataAnnotations;

namespace Inventory.DTOs
{
    /// <summary>
    /// Represents the data transfer object for user login, containing the user's email and password.
    /// </summary>
    public sealed class LoginDto
    {
        /// <summary>
        /// Gets or sets the user's email address. This property is required and must be a valid email format.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password. This property is required and must have a minimum length of 8 characters.
        /// </summary>
        [Required]
        public string Password { get; set; }

    }
}
