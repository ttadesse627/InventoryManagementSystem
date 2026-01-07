using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Contracts.RequestDtos;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Infrastructure.Contexts;

namespace OrderMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(WarehouseDbContext context, IUserService userService) : ControllerBase
{
    private readonly WarehouseDbContext _context = context;
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest registerRequest)
    {
        return Created("", await _userService.CreateUserAsync(registerRequest, CancellationToken.None));
    }
    [Authorize]
    [HttpPut("{id}/update")]
    public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateRequest updateRequest)
    {
        return Ok(await _userService.UpdateUser(id, updateRequest));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/delete")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
    {
        return Ok(await _userService.DeleteUserAsync(id, CancellationToken.None));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet(Name = "Users")]
    public async Task<ActionResult<UserDto>> Get(
        int currentPage = 1,
        int pageSize = 15,
        string? sortBy = null,
        bool sortDescending = false)
    {
        return Ok(await _userService.GetUsersAsync(currentPage, pageSize, sortBy, sortDescending));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        return Ok(await _userService.GetUserByIdAsync(id, CancellationToken.None));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        return Ok(await _userService.GetUserProfileAsync(CancellationToken.None));
    }

}