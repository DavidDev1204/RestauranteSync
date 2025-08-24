using RestauranteSync.Domain.Entities;

namespace RestauranteSync.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
}
