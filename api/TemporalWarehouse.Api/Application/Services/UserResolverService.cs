using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TemporalWarehouse.Api.Application.AppServices.Interfaces;

namespace TemporalWarehouse.Api.Application.Services;

public class UserResolverService(IHttpContextAccessor httpContext) : IUserResolverService
{
    private readonly IHttpContextAccessor _httpContext = httpContext;

    public string? GetUserEmail()
    {
        return _httpContext.HttpContext?.User?.Claims?
                .SingleOrDefault(p => p.Type == JwtRegisteredClaimNames.Email)?.Value;
    }

    public string? GetTokenExpirationTime()
    {
        return _httpContext.HttpContext?.User?.Claims?
                .SingleOrDefault(p => p.Type == JwtRegisteredClaimNames.Exp)?.Value;
    }

    public string? GetTokenId()
    {
        return _httpContext.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    }

    public Guid GetUserId()
    {
        var userIdClaim = _httpContext.HttpContext?.User?.Claims?.SingleOrDefault(p => p.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return userId;
        }
        return Guid.Empty;
    }

    public string? GetLocale()
    {
        if (_httpContext.HttpContext != null && _httpContext.HttpContext.Request.Query.ContainsKey("locale"))
        {
            return _httpContext.HttpContext.Request.Query["locale"].ToArray()[0];
        }

        return string.Empty;
    }

    public Guid GetId()
    {
        return new Guid();
    }
}
