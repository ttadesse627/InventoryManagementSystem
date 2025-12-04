
namespace TemporalWarehouse.Api.Contracts.RequestDtos;

public record CreateProductRequest
{
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
    public int InitialQuantity { get; set; }
}