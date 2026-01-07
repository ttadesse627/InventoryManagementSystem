

namespace TemporalWarehouse.Api.Models.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; }

    public ApplicationUser? User { get; set; }
}
