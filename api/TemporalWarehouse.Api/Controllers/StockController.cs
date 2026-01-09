

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Contracts.RequestDtos;

namespace TemporalWarehouse.Api.Controllers;

[ApiController]
[Route("api/products/{id:guid}")]
public class StockController(IStockService stockService) : ControllerBase
{
    private readonly IStockService _stockService = stockService;

    [Authorize(Roles = "Admin, Seller")]
    [HttpPost("add-stock")]
    public async Task<IActionResult> AddStock(Guid productId, [FromBody] AddStockRequest request)
    {
        try
        {
            await _stockService.AddStockAsync(productId, request.Quantity);
            return Ok(new { Message = "Stock added successfully." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Product not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, Seller")]
    [HttpPost("remove-stock")]
    public async Task<IActionResult> RemoveStock(Guid productId, [FromBody] RemoveStockRequest request)
    {
        try
        {
            await _stockService.RemoveStockAsync(productId, request.Quantity);
            return Ok(new { Message = "Stock removed successfully." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Product not found.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
