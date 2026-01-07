

using System.Linq.Expressions;
using TemporalWarehouse.Api.Contracts.RequestDtos;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Application.Interfaces;

public interface IUserService
{
    Task<ApiResponse<AuthResponse>> CreateUserAsync(RegisterRequest registerRequest, CancellationToken cancellationToken);
    Task<ApiResponse<string>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserDetailDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserProfileDto?> GetUserProfileAsync(CancellationToken cancellationToken);
    Task<PaginatedResult<UserDto>> GetUsersAsync(int currentPage, int pageSize, string? sortBy, bool sortDescending, Expression<Func<ApplicationUser, bool>>? filter = null)
;
    Task<ApiResponse<string>> UpdateUser(Guid id, UpdateRequest request);
    Task<ApiResponse<string>> UpdateUserProfile(UpdateProfileRequest request, CancellationToken cancellationToken);
}