

using System.ComponentModel.DataAnnotations;

namespace TemporalWarehouse.Api.Models.Entities;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;

    public decimal Price { get; set; }

    public int CurrentQuantity { get; set; }

    // Needed for concurrency conflict resolution
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public List<StockTransaction> StockTransactions { get; set; } = new();
}