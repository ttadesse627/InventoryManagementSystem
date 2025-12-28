using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Models.Entities;


namespace TemporalWarehouse.Api.Application.Services;

public class HistoryService(IStockRepository stockRepository) : IHistoryService
{
    private readonly IStockRepository _stockRepository = stockRepository;

    public async Task<List<StockTransaction>> GetHistoryAsync(Guid productId)
    {
        return await _stockRepository.GetByProductAsync(productId);
    }

    public async Task<int> GetStockAtTimeAsync(Guid productId, DateTime timestamp)
    {
        timestamp = DateTime.SpecifyKind(timestamp, DateTimeKind.Local).ToUniversalTime();
        var transaction = await _stockRepository.GetBeforeAsync(productId, timestamp);

        return transaction?.NewTotal ?? 0;
    }
}