

using Microsoft.AspNetCore.Mvc;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Contracts.ResponseDtos;

namespace TemporalWarehouse.Api.Controllers;

[ApiController]
[Route("api/products/{id:guid}/history")]
public class HistoryController(IHistoryService historyService, IProductService productService) : ControllerBase
{
    private readonly IHistoryService _historyService = historyService;
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<ActionResult<List<StockTransactionResponse>>> GetHistory(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound("Product not found.");

        var history = await _historyService.GetHistoryAsync(id);

        return history.Select(t => new StockTransactionResponse
        {
            OccurredAt = t.OccurredAt,
            Type = t.Type,
            QuantityChanged = t.QuantityChanged,
            NewTotal = t.NewTotal
        }).ToList();
    }

    [HttpGet("stock-at")]
    public async Task<ActionResult<int>> GetStockAt(Guid productId, [FromQuery] DateTime at)
    {
        var product = await _productService.GetByIdAsync(productId);

        Console.WriteLine(at);
        if (product == null)
            return NotFound("Product not found.");

        var value = await _historyService.GetStockAtTimeAsync(productId, at);

        return Ok(new { Quantity = value });
    }
}
