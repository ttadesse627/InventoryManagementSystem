using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace TemporalWarehouse.Api.Infrastructure.Contexts;

public class WarehouseDbContextFactory
    : IDesignTimeDbContextFactory<WarehouseDbContext>
{
    public WarehouseDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddEnvironmentVariables()
            .Build();

        var migrationConn = config
            .GetConnectionString("NpgsqlConnectionMigration");

        var options = new DbContextOptionsBuilder<WarehouseDbContext>()
            .UseNpgsql(migrationConn, o =>
            {
                o.CommandTimeout(120);
            })
            .Options;

        return new WarehouseDbContext(options);
    }
}
