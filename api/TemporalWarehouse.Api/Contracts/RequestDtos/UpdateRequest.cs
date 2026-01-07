
namespace TemporalWarehouse.Api.Contracts.RequestDtos;

public record UpdateRequest
{
    public string? FirstName { get; init; } = null;
    public string? LastName { get; init; } = null;
    public string? Address { get; init; } = null;
    public IList<string> Roles { get; init; } = [];
};