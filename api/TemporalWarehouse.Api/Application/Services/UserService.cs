using System.Linq.Expressions;
using FluentValidation;
using TemporalWarehouse.Api.Application.AppServices.Interfaces;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Application.Services.Validators;
using TemporalWarehouse.Api.Contracts.RequestDtos;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Enums;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Application.Services;

public class UserService(
                            IIdentityService identityService,
                            ITokenGeneratorService tokenGeneratorService,
                            IUserResolverService userResolverService) : IUserService
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenGeneratorService _tokenGeneratorService = tokenGeneratorService;
    private readonly IUserResolverService _userResolverService = userResolverService;

    public async Task<ApiResponse<AuthResponse>> CreateUserAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var userValidator = new CreateUserCommandValidator();

        await userValidator.ValidateAsync(registerRequest, cancellationToken);

        var user = new ApplicationUser
        {
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email,
            UserName = registerRequest.Email,
            Address = registerRequest.Address
        };

        bool rolesWereEmpty = !registerRequest.Roles.Any();


        if (rolesWereEmpty)
        {
            registerRequest.Roles.Add("User");
        }

        var createResult = await _identityService.CreateUserAsync(
                            user,
                            registerRequest.Roles,
                            registerRequest.Password
                        );

        if (!createResult.Success)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = string.Join(", ", createResult.Errors)
            };
        }

        var currentUserId = _userResolverService.GetUserId();
        var apiResponse = new ApiResponse<AuthResponse>()
        {
            Success = true,
            Message = currentUserId == Guid.Empty ? "You have registered successfully." : "User created successfully."
        };
        if (currentUserId == Guid.Empty)
        {
            apiResponse.Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = await _tokenGeneratorService.GenerateTokenAsync(user),
                RefreshToken = _tokenGeneratorService.GenerateRefreshToken()
            };
        }

        return apiResponse;
    }

    public async Task<ApiResponse<string>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        ApiResponse<string> apiResponse = new();

        if (userId == Guid.Empty)
        {
            throw new ValidationException("User doesn't exist");
        }

        ApplicationUser? user = await _identityService.GetByIdAsync(userId) ??
                                throw new ValidationException("User doesn't exist");

        user.Status = UserStatus.Deleted;


        if (await _identityService.UpdateUserAsync(user))
        {
            apiResponse.Success = true;
            apiResponse.Data = "Operation Succeeded!";
            apiResponse.Message = "User Deleted successfully!";
        }

        return apiResponse;
    }

    public async Task<PaginatedResult<UserDto>> GetUsersAsync(int currentPage, int pageSize, string? sortBy, bool sortDescending, Expression<Func<ApplicationUser, bool>>? filter = null)
    {
        return await _identityService.GetPaginatedAsync(currentPage, pageSize, sortBy, sortDescending);
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {

        var user = await _identityService.GetByIdAsync(userId) ?? throw new KeyNotFoundException("This user does not exist.");
        IReadOnlyList<string> userRoles = user is not null ? await _identityService.GetUserRolesAsync(user) : [];

        var userDto = new UserDetailDto
        {
            Id = user!.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Address = user.Address,
            Roles = userRoles
        };

        return userDto;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(CancellationToken cancellationToken)
    {
        var currentUserId = _userResolverService.GetUserId();
        var user = await _identityService.GetByIdAsync(currentUserId);

        return new UserProfileDto
        {
            Id = user!.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Address = user.Address,
        };
    }


    public async Task<ApiResponse<string>> UpdateUser(Guid id, UpdateRequest request)
    {
        ApiResponse<string> apiResponse = new();

        if (id == Guid.Empty)
        {
            throw new ValidationException("User doesn't exist");
        }

        ApplicationUser? user = await _identityService.GetByIdAsync(id) ??
                                throw new ValidationException("User doesn't exist");


        if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.Address)) user.Address = request.Address;

        if (request.Roles.Any())
        {
            await _identityService.UpdateUserRolesAsync(user, request.Roles);
        }

        if (await _identityService.UpdateUserAsync(user))
        {
            apiResponse.Success = true;
            apiResponse.Data = "Operation Succeeded!";
            apiResponse.Message = "User updated successfully!";
        }

        return apiResponse;
    }

    public async Task<ApiResponse<string>> UpdateUserProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        ApiResponse<string> apiResponse = new();
        var currentUserId = _userResolverService.GetUserId();
        if (currentUserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }

        ApplicationUser? user = await _identityService.GetByIdAsync(currentUserId) ??
                                throw new ValidationException("User doesn't exist");


        if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName)) user.FirstName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.Address)) user.FirstName = request.Address;

        if (await _identityService.UpdateUserAsync(user))
        {
            apiResponse.Success = true;
            apiResponse.Data = "Operation Succeeded!";
            apiResponse.Message = "User updated successfully!";
        }

        return apiResponse;
    }
}