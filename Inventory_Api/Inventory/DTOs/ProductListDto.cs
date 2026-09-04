namespace Inventory.DTOs
{
    public class ProductListDto
    {
        public List<ProductResponseDto> Products { get; set; } = new List<ProductResponseDto>();

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }
    }
}
