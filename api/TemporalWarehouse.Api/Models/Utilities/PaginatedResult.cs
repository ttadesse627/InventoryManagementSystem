
namespace TemporalWarehouse.Api.Models.Utilities;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
