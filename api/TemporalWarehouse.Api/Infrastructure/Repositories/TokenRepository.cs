using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TemporalWarehouse.Api.Application.AppServices.Interfaces;
using TemporalWarehouse.Api.Models.Entities;
using StackExchange.Redis;
using TemporalWarehouse.Api.Infrastructure.Contexts;

namespace TemporalWarehouse.Api.Infrastructure.Persistence;

public class TokenRepository(IConnectionMultiplexer redis,
                                IServiceScopeFactory scopeFactory,
                                WarehouseDbContext context) : ITokenRepository
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly WarehouseDbContext _context = context;
    private readonly IDatabase _redisDb = redis.GetDatabase();

    public async Task SaveTokenAsync(RefreshToken token, Guid userId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        dbContext.Add(token);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        await dbContext.RefreshTokens.Where(token => token.UserId == userId).ExecuteDeleteAsync(cancellationToken);
    }
    public async Task RevokeTokensAsync(string jti, TimeSpan expirationTime)
    {
        await _redisDb.StringSetAsync($"revoked_jti: {jti}", true, expirationTime);
    }
    public async Task<bool> IsTokenRevokedAsync(string jti)
    {
        return await _redisDb.KeyExistsAsync($"revoked_jti: {jti}");
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Where(token => token.Token == refreshToken)
            .FirstOrDefaultAsync();

        return token;
    }
}