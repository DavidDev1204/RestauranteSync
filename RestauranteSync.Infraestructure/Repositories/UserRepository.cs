using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using RestauranteSync.Domain.Entities;
using RestauranteSync.Domain.Repositories;
using RestauranteSync.Infraestructure.Configuration;

namespace RestauranteSync.Infraestructure.Repositories;

public class UserRepository(IOptions<DatabaseSettings> databaseSettings) : IUserRepository
{
    private readonly string _connectionString = databaseSettings != null
        && databaseSettings.Value != null
        && !string.IsNullOrEmpty(databaseSettings.Value.ToString())
        ? DatabaseSettings.ConvertPostgresUrl(databaseSettings.Value.PostgresConnection)
        : throw new Exception("No se encontró cadena de conexión configurada");

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            SELECT usuario, password, nombre, tipousuario, servidor 
            FROM usuario 
            WHERE usuario = @Username";
        
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = "SELECT COUNT(1) FROM usuario WHERE usuario = @Username";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Username = username });
        return count > 0;
    }
}
