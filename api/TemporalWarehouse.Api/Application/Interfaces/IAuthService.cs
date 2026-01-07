using TemporalWarehouse.Api.Contracts.RequestDtos;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> Login(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> LoginWithRefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task Logout(CancellationToken cancellationToken);
}