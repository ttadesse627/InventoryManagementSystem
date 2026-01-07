
namespace TemporalWarehouse.Api.Contracts.RequestDtos;

public record UpdateProfileRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Address { get; init; }
};