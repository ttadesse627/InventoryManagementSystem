using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TemporalWarehouse.Api.Application.AppServices.Interfaces;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Infrastructure.Persistence;

public class TokenGeneratorService(IOptions<JwtSettings> options, UserManager<ApplicationUser> userManager) : ITokenGeneratorService
{
    private readonly JwtSettings _jwtSettings = options.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var secretKey = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey ??
                        throw new InvalidOperationException("JWT Key not configured."));

        IList<Claim> claims = await _userManager.GetClaimsAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return await Task.FromResult(token);
    }

    public async Task<string> GenerateTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return await GenerateTokenAsync(user ?? throw new KeyNotFoundException("User doesn't exist"));
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}