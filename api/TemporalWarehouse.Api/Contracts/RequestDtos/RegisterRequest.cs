

namespace TemporalWarehouse.Api.Contracts.RequestDtos;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Address,
    IList<string> Roles,
    string Email,
    string Password
);

