

using TemporalWarehouse.Api.Application.AppServices.Interfaces;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Contracts.RequestDtos;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Application.Services;

public class AuthService(
                        IIdentityService identityService,
                        ITokenGeneratorService tokenGeneratorService,
                        ITokenRepository tokenRepository,
                        IUserResolverService userResolverService,
                        IServiceProvider serviceProvider,
                        IBackgroundTaskQueue taskQueue) : IAuthService
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenGeneratorService _tokenGeneratorService = tokenGeneratorService;
    private readonly ITokenRepository _tokenRepository = tokenRepository;
    private readonly IUserResolverService _userResolverService = userResolverService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IBackgroundTaskQueue _taskQueue = taskQueue;

    public async Task<Result<AuthResponse>> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(loginRequest.Email))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(loginRequest.Email));
        }

        if (string.IsNullOrEmpty(loginRequest.Password))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(loginRequest.Password));
        }
        var isAuthenticated = await _identityService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);
        if (!isAuthenticated)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var user = await _identityService.GetUserByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            throw new KeyNotFoundException("Invalid email or password.");
        }

        var token = await _tokenGeneratorService.GenerateTokenAsync(user);
        var refreshToken = _tokenGeneratorService.GenerateRefreshToken();

        AuthResponse authResponse = new()
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            RefreshToken = refreshToken
        };

        // Save refresh token in background
        _taskQueue.QueueBackgroundTaskItem(async cancellationToken =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
            RefreshToken refToken = new()
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(3),
            };

            await _tokenRepository.SaveTokenAsync(refToken, user.Id, cancellationToken);
        });

        return Result<AuthResponse>.SetValue(authResponse);
    }

    public async Task<Result<AuthResponse>> LoginWithRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentNullException(refreshToken, nameof(refreshToken));
        }

        var existingToken = await _tokenRepository.GetRefreshTokenAsync(refreshToken);
        if (existingToken == null || existingToken.Expires < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var user = await _identityService.GetByIdAsync(existingToken.UserId);

        var token = await _tokenGeneratorService.GenerateTokenAsync(user ?? throw new UnauthorizedAccessException("This user is not authorized yet."));
        var refToken = _tokenGeneratorService.GenerateRefreshToken();

        AuthResponse authResponse = new()
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            RefreshToken = refreshToken
        };

        // Save refresh token in background
        _taskQueue.QueueBackgroundTaskItem(async cancellationToken =>
        {
            var repository = _serviceProvider.GetRequiredService<ITokenRepository>();

            await _tokenRepository.RemoveTokensAsync(user.Id, cancellationToken);
            RefreshToken refToken = new()
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(3),
            };

            await _tokenRepository.SaveTokenAsync(refToken, user.Id, cancellationToken);
        });

        return Result<AuthResponse>.SetValue(authResponse);
    }

    public async Task Logout(CancellationToken cancellationToken)
    {
        var userId = _userResolverService.GetUserId();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException("You are not authorized yet");

        await _tokenRepository.RemoveTokensAsync(userId, cancellationToken);

        var jti = _userResolverService.GetTokenId();
        if (!string.IsNullOrEmpty(jti))
        {
            // calculate time until token expiry:
            var expString = _userResolverService.GetTokenExpirationTime();
            if (long.TryParse(expString, out var exp))
            {
                var expiry = DateTimeOffset.FromUnixTimeSeconds(exp)
                                .UtcDateTime - DateTime.UtcNow;
                await _tokenRepository.RevokeTokensAsync(jti, expiry);
            }
        }
    }
}