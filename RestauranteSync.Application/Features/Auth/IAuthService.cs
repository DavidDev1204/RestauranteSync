using RestauranteSync.Domain.Entities;

namespace RestauranteSync.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
