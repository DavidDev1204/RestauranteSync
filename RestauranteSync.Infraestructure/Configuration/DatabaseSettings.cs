namespace RestauranteSync.Infraestructure.Configuration;

public class DatabaseSettings
{
    public string PostgresConnection { get; set; } = string.Empty;

    // Método para convertir URL de Railway
    public static string ConvertPostgresUrl(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var password = uri.UserInfo.Split(':')[1];
        var host = uri.Host;
        var port = uri.Port;

        return $"Host={host};Port={port};Database={db};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
}
