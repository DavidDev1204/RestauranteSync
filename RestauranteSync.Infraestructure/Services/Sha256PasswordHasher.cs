using System.Security.Cryptography;
using System.Text;
using RestauranteSync.Domain.Interfaces;

namespace RestauranteSync.Infraestructure.Services;

public class Sha256PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == passwordHash;
    }
}
