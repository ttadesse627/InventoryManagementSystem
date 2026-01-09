using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using TemporalWarehouse.Api.Application.AppServices.Interfaces;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Application.Services;
using TemporalWarehouse.Api.Infrastructure.Contexts;
using TemporalWarehouse.Api.Infrastructure.Persistence;
using TemporalWarehouse.Api.Infrastructure.Repositories;
using TemporalWarehouse.Api.Models.Entities;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api;

public static class DependencyContainer
{
    public static IServiceCollection AddServicesRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);
        services.ConfigureIdentity(configuration);
        services.AddRepositories();

        services.AddOptions<AppMetadata>()
                .Bind(configuration.GetSection("AppMetadata"))
                .ValidateDataAnnotations()
                .Validate(x =>
                    !string.IsNullOrWhiteSpace(x.Api.Platform) &&
                    !string.IsNullOrWhiteSpace(x.Database.Platform),
                    "Hosting Information is not fully configured.")
                .ValidateOnStart();

        return services;
    }

    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NpgsqlRemoteConnection")
            ?? throw new InvalidOperationException("Connection string for PostgreSQL not found.");

        var redisConnectionString = configuration.GetConnectionString("RedisConnection")
            ?? throw new InvalidOperationException("Connection string for Redis not found.");

        services.AddDbContext<WarehouseDbContext>(options =>
        {

            options.UseNpgsql(connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                                                    maxRetryCount: 5,
                                                    maxRetryDelay: TimeSpan.FromSeconds(10),
                                                    errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(60);
                }).EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);


        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = ConfigurationOptions.Parse(redisConnectionString);
            options.AbortOnConnectFail = false;
            options.ConnectRetry = 5;
            options.ConnectTimeout = 5000;

            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }

    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsSection);
        var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? new JwtSettings();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.User.AllowedUserNameCharacters = "";
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<WarehouseDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.RequireHttpsMetadata = false;
                jwtOptions.SaveToken = true;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = jwtSettings.ExpiryInMinutes > 0
                        ? TimeSpan.FromMinutes(jwtSettings.ExpiryInMinutes)
                        : TimeSpan.Zero
                };

                jwtOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        if (string.IsNullOrEmpty(jti))
                        {
                            context.Fail("Unknown token.");
                            return;
                        }

                        var revokationService = context.HttpContext.RequestServices.GetRequiredService<ITokenRepository>();
                        if (await revokationService.IsTokenRevokedAsync(jti))
                        {
                            context.Fail("This token has been revoked prreviously.");
                        }
                    }
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserResolverService, UserResolverService>();
        services.AddTransient<ITokenGeneratorService, TokenGeneratorService>();
        services.AddTransient<ITokenRepository, TokenRepository>();

        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IStockRepository, StockRepository>();
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<IStockService, StockService>();
        services.AddTransient<IHistoryService, HistoryService>();

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<QueuedHostedService>();

        return services;
    }
}
