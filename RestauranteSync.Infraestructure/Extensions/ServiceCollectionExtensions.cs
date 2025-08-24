using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RestauranteSync.Application.Features.Auth;
using RestauranteSync.Application.Features.SyncDatabase;
using RestauranteSync.Domain.Interfaces;
using RestauranteSync.Domain.Repositories;
using RestauranteSync.Infraestructure.Configuration;
using RestauranteSync.Infraestructure.Repositories;
using RestauranteSync.Infraestructure.Services;
using System.Text;

namespace RestauranteSync.Infraestructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        SQLitePCL.Batteries.Init();

        // Configure settings
        services.Configure<DatabaseSettings>(configuration.GetSection("ConnectionStrings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        // Validate JWT Settings
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() 
            ?? throw new InvalidOperationException("JwtSettings configuration is missing or invalid");

        if (string.IsNullOrEmpty(jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is not configured");
        
        if (string.IsNullOrEmpty(jwtSettings.Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured");
        
        if (string.IsNullOrEmpty(jwtSettings.Audience))
            throw new InvalidOperationException("JWT Audience is not configured");
        
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        // Validate Database Settings
        var dbSettings = configuration.GetSection("ConnectionStrings").Get<DatabaseSettings>();
        if (dbSettings == null)
        {
            throw new InvalidOperationException("Database configuration is missing or invalid");
        }
        
        if (string.IsNullOrEmpty(dbSettings.PostgresConnection))
        {
            throw new InvalidOperationException("Database connection string is not configured");
        }
        
        // Register repositories
        services.AddScoped<ISyncRepository, SyncRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register infrastructure services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();
        
        // Register application services
        services.AddScoped<ISyncDatabaseService, SyncDatabaseService>();
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}
