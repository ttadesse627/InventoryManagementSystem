


using Microsoft.AspNetCore.Identity;
using TemporalWarehouse.Api.Models.Enums;

namespace TemporalWarehouse.Api.Models.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.Active;
}