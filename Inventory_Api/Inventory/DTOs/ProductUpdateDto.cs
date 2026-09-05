using System.ComponentModel.DataAnnotations;

namespace Inventory.DTOs
{
    public class ProductUpdateDto
    {
        [Required] 
        [Range(1, int.MaxValue)] // validation to ensure Id is positive
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)] // validation to ensure price is non-negative
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
