using System.Linq.Expressions;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Application.AppServices.Interfaces;

public interface IIdentityService
{
    Task<ApiResponse<int>> CreateUserAsync(ApplicationUser user, IList<string> roles, string password);
    Task<bool> AuthenticateUserAsync(string email, string password);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetByIdAsync(Guid id);
    Task<bool> CreateRolesAsync(IList<string> roleNames);
    Task<bool> UpdateUserRolesAsync(ApplicationUser user, IList<string> roles);
    Task<bool> UpdateUserAsync(ApplicationUser user);
    Task<IReadOnlyList<string>> GetUserRolesAsync(ApplicationUser user);
    Task<bool> IsInRoleAsync(Guid userId, string role);
    Task<bool> AssignUserToRole(Guid userId, IList<string> roles);
    Task SeedIdentitiesAsync(IServiceProvider services);
    Task<PaginatedResult<UserDto>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        Expression<Func<ApplicationUser, bool>>? filter = null);

}