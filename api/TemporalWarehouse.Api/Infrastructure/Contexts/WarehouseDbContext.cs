using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemporalWarehouse.Api.Models.Entities;


namespace TemporalWarehouse.Api.Infrastructure.Contexts;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
                                : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
                                                    IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
                                                    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}