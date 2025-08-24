using RestauranteSync.Domain.Entities;
using RestauranteSync.Domain.Interfaces;
using RestauranteSync.Domain.Repositories;

namespace RestauranteSync.Application.Features.Auth;

public class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var token = tokenService.GenerateToken(user);
        var expiresAt = tokenService.GetExpirationTime();

        return new AuthResponse
        {
            Token = token,
            Username = user.Nombre,
            ExpiresAt = expiresAt
        };
    }
}
