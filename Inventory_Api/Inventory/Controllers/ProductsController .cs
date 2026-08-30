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

    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _service.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _service.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(
        ProductCreateDto dto,
        CancellationToken cancellationToken)
    {
        var product = await _service.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        ProductUpdateDto dto,
        CancellationToken cancellationToken)
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
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