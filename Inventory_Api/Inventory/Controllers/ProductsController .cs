using Inventory.DTOs;
using Inventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers;

// Handles HTTP requests. Calls the services.
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    // test, no DB call
    //[HttpGet]
    //public IActionResult GetProducts()
    //{
    //    return Ok(new[]
    //    {
    //        new { Id = 1, Name = "Laptop", Price = 999 },
    //        new { Id = 2, Name = "Keyboard", Price = 49 }
    //    });
    //}

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <returns>The list of all products.</returns>
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var products = await _service.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The requested product.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _service.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Creates a product.
    /// </summary>
    /// <param name="dto">The product data.</param>
    /// <returns>The created product.</returns>
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(
        ProductCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _service.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    /// <summary>
    /// Updates a product by its ID.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="dto">The product data.</param>
    /// <returns>A valid action result.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        ProductUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _service.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>A valid action result.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _service.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}