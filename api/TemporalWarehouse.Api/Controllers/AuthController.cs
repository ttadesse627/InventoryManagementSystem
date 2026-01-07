using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Contracts.RequestDtos;
using TemporalWarehouse.Api.Contracts.ResponseDtos;

namespace TemporalWarehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        return Ok(await _authService.Login(loginRequest, CancellationToken.None));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] string refreshToken)
    {
        return Ok(await _authService.LoginWithRefreshToken(refreshToken, CancellationToken.None));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await _authService.Logout(CancellationToken.None);
        return NoContent();
    }
}