
namespace TemporalWarehouse.Api.Models.ConstantValues;

public record IdentityConstantValues
{
    public const string ADMIN_EMAIL = "Admin@test.com";
    public const string ADMIN_PASSWORD = "Admin123!";

    public sealed record SystemRoles
    {
        public const string ADMIN = "Admin";
        public const string SELLER = "Seller";
        public const string CUSTOMER = "Customer";

        public static readonly IReadOnlyList<string> ALL = [ADMIN, SELLER, CUSTOMER];
    }
}