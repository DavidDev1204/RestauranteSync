using RestauranteSync.Domain.Entities;

namespace RestauranteSync.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    DateTime GetExpirationTime();
}
