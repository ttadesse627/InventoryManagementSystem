using System.Linq.Expressions;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using TemporalWarehouse.Api.Application.AppServices.Interfaces;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Infrastructure.Contexts;
using TemporalWarehouse.Api.Models.ConstantValues;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Infrastructure.Persistence
{
    public class IdentityService(
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole<Guid>> roleManager,
                WarehouseDbContext context)
                : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
        private readonly WarehouseDbContext _context = context;

        public async Task<ApiResponse<int>> CreateUserAsync(ApplicationUser user, IList<string> roles, string password)
        {
            var response = new ApiResponse<int>();

            ArgumentNullException.ThrowIfNull(user);
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            var existingUser = await _userManager.FindByEmailAsync(user.Email!);
            if (existingUser != null)
            {
                response.Message += "user with the given email already exists";
                throw new ValidationException("user with the given email already exists");
            }

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                response.Errors = [.. result.Errors.Select(er => er.Description)];
                throw new InvalidOperationException(string.Join("; ", result.Errors));
            }

            if (roles.Count != 0)
            {
                await _userManager.AddToRolesAsync(user, roles);
            }

            List<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            ];

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r ?? string.Empty)));


            var claimResult = await _userManager.AddClaimsAsync(user, claims);
            if (!claimResult.Succeeded)
            {
                response.Errors.AddRange(claimResult.Errors.Select(err => err.Description));
                await _userManager.DeleteAsync(user);
            }

            response.Success = true;

            return response;
        }

        public async Task<bool> AuthenticateUserAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Email or password cannot be null or empty.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            return isPasswordValid;
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));

            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            return await _userManager.Users
                        .Where(user => user.Id == id)
                        .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateRolesAsync(IList<string> roles)
        {
            if (!roles.Any())
                throw new ArgumentNullException(nameof(roles));

            foreach (var role in roles)
            {
                if (string.IsNullOrWhiteSpace(role))
                    continue;

                var exists = await _roleManager.RoleExistsAsync(role);
                if (exists)
                    continue;

                var roleEntity = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = role.ToUpperInvariant()
                };

                var result = await _roleManager.CreateAsync(roleEntity);
                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<IReadOnlyList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return [.. await _userManager.GetRolesAsync(user)];
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentNullException(nameof(role));

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AssignUserToRole(Guid userId, IList<string> roles)
        {
            List<string> errors = [];

            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId), "User ID cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId.ToString()) ??
                throw new ArgumentException("User not found.", nameof(userId));

            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    var isRoleExist = await _roleManager.RoleExistsAsync(role);

                    if (!isRoleExist) errors.Add($"Role with name '{role}' not found.");

                    var result = await _userManager.AddToRoleAsync(user, role ?? string.Empty);
                    if (!result.Succeeded)
                    {
                        return false;
                    }
                }
            }

            var result2 = await _userManager.AddToRoleAsync(user, "User");
            if (!result2.Succeeded)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateUserRolesAsync(ApplicationUser user, IList<string> roles)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user), "Invalid user!");

            var oldRoles = await _userManager.GetRolesAsync(user);
            var identityResult = await _userManager.RemoveFromRolesAsync(user, oldRoles);
            if (!identityResult.Succeeded)
            {
                return identityResult.Succeeded;
            }

            if (roles.Any())
            {
                var filteredRoles = await GetFilteredRoles(roles);
                identityResult = await _userManager.AddToRolesAsync(user, filteredRoles);
            }

            return identityResult.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private async Task<IList<string>> GetFilteredRoles(IList<string> roles)
        {
            IList<string> existingRoles = [];

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    existingRoles.Add(role);
                }
            }

            return existingRoles;
        }
        public async Task SeedIdentitiesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Roles seeding
            foreach (var role in IdentityConstantValues.SystemRoles.ALL)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var adminUser = await userManager.FindByEmailAsync(IdentityConstantValues.ADMIN_EMAIL);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = IdentityConstantValues.ADMIN_EMAIL,
                    Email = IdentityConstantValues.ADMIN_EMAIL,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, IdentityConstantValues.ADMIN_PASSWORD);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Admin user creation failed: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, IdentityConstantValues.SystemRoles.ADMIN))
            {
                await userManager.AddToRoleAsync(adminUser, IdentityConstantValues.SystemRoles.ADMIN);
            }

            var claims = await userManager.GetClaimsAsync(adminUser);

            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == adminUser.Id.ToString()))
            {
                await userManager.AddClaimsAsync(adminUser,
                    [
                        new(JwtRegisteredClaimNames.Sub, adminUser.Id.ToString()),
                        new(JwtRegisteredClaimNames.Email, IdentityConstantValues.ADMIN_EMAIL),
                        new(ClaimTypes.Role, IdentityConstantValues.SystemRoles.ADMIN),
                    ]);
            }
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        Expression<Func<ApplicationUser, bool>>? filter = null)
        {
            IQueryable<ApplicationUser> query = _context.Set<ApplicationUser>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = ApplySorting(query, sortBy, sortDescending);
            }

            // Total count
            int totalCount = await query.CountAsync();

            // Pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedResult<UserDto>
            {
                Items = items.Select(u =>
                                new UserDto
                                {
                                    Id = u.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Email = u.Email,
                                    Address = u.Address
                                }
                            )
                            .ToList(),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<ApplicationUser> ApplySorting(
            IQueryable<ApplicationUser> query,
            string sortBy,
            bool sortDescending)
        {
            var parameter = Expression.Parameter(typeof(ApplicationUser), "x");
            var property = Expression.PropertyOrField(parameter, sortBy);
            var lambda = Expression.Lambda(property, parameter);

            string methodName = sortDescending ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .First(m =>
                    m.Name == methodName &&
                    m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(ApplicationUser), property.Type)
                .Invoke(null, [query, lambda]);

            return (IQueryable<ApplicationUser>)result!;
        }

    }
}