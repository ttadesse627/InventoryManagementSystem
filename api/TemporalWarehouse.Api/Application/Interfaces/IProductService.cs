


using TemporalWarehouse.Api.Models.Entities;

namespace TemporalWarehouse.Api.Application.Interfaces;

public interface IProductService
{
    public Task<Product> CreateAsync(string name, string sku, decimal price, int initialQuantity);

    public Task<List<Product>> GetAllAsync();

    public Task<Product?> GetByIdAsync(Guid id);

    public Task UpdateAsync(Guid id, string name, string sku, decimal price);

    public Task DeleteAsync(Guid id);
}