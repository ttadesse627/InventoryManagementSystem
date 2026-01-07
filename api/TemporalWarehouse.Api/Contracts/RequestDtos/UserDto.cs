

namespace TemporalWarehouse.Api.Contracts.ResponseDtos;

public record UserDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
}

