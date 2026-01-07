

using TemporalWarehouse.Api.Models.Entities;

namespace TemporalWarehouse.Api.Application.AppServices.Interfaces;

public interface ITokenGeneratorService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
    string GenerateRefreshToken();
}