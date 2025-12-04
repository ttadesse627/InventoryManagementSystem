using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Models.Entities;


namespace TemporalWarehouse.Api.Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<Product> CreateAsync(string name, string sku, decimal price, int initialQuantity)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            SKU = sku,
            Price = price,
            CurrentQuantity = initialQuantity
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product;
    }

    public Task<List<Product>> GetAllAsync()
        => _productRepository.GetAllAsync();

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _productRepository.GetByIdAsync(id);

    public async Task UpdateAsync(Guid id, string name, string sku, decimal price)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Product not found.");

        product.Name = name;
        product.SKU = sku;
        product.Price = price;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Product not found.");

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();
    }
}