

namespace TemporalWarehouse.Api.Contracts.ResponseDtos;

public record ApiResponse<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = [];
    public bool Success { get; set; } = false;
}