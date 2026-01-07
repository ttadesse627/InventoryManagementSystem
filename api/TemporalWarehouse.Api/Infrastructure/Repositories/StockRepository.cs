
using Microsoft.EntityFrameworkCore;
using TemporalWarehouse.Api.Infrastructure.Contexts;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Application.Interfaces;

namespace TemporalWarehouse.Api.Infrastructure.Repositories;

public class StockRepository(WarehouseDbContext context) : IStockRepository
{
    private readonly WarehouseDbContext _context = context;

    public async Task AddAsync(StockTransaction transaction)
    {
        await _context.StockTransactions.AddAsync(transaction);
    }

    public Task<List<StockTransaction>> GetByProductAsync(Guid productId)
    {
        return _context.StockTransactions
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.OccurredAt)
            .ToListAsync();
    }

    public async Task<StockTransaction?> GetBeforeAsync(Guid productId, DateTime dateTime)
    {
        var utcTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
        return await _context.StockTransactions
        .AsNoTracking()
        .Where(s => s.ProductId == productId && s.OccurredAt <= utcTime)
        .OrderByDescending(s => s.OccurredAt)
        .FirstOrDefaultAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}