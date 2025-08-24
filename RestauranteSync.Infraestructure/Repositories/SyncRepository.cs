using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Npgsql;
using RestauranteSync.Domain.Entities;
using RestauranteSync.Domain.Repositories;
using RestauranteSync.Infraestructure.Configuration;

namespace RestauranteSync.Infraestructure.Repositories;

public class SyncRepository(IOptions<DatabaseSettings> databaseSettings) : ISyncRepository
{
    private readonly string _postgresConnectionString = databaseSettings != null 
        && databaseSettings.Value != null 
        && !string.IsNullOrEmpty(databaseSettings.Value.ToString()) 
        ? DatabaseSettings.ConvertPostgresUrl(databaseSettings.Value.PostgresConnection)
        : throw new Exception("No se encontró cadena de conexión configurada");

    private readonly string _sqliteConnectionString = "Data Source=./temp/Database.db;Mode=ReadWriteCreate;Cache=Shared;Pooling=true;";

    public async Task<List<string>> GetTableNamesAsync(CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_postgresConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new NpgsqlCommand(
            "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';",
            connection
        );

        var tableNames = new List<string>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
            tableNames.Add(reader.GetString(0));

        return tableNames;
    }

    public async Task<List<ColumnInfo>> GetColumnsByTableNameAsync(string tableName, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_postgresConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new NpgsqlCommand(
            $"SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '{tableName}';",
            connection
        );

        var columns = new List<ColumnInfo>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
            columns.Add(new ColumnInfo { Name = reader.GetString(0), DataType = reader.GetString(1) });

        return columns;
    }

    public async Task<List<List<string>>> GetDataByTableNameAsync(string tableName, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_postgresConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new NpgsqlCommand($"SELECT * FROM {tableName};", connection);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var values = new List<List<string>>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var rowValues = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.IsDBNull(i) ? "NULL" : $"'{reader.GetValue(i).ToString()?.Replace("'", "''")}'";
                rowValues.Add(value);
            }
            values.Add(rowValues);
        }

        return values;
    }

    public async Task<bool> CreateSqliteDatabaseAsync(List<DatabaseInfo> databaseInfos, CancellationToken cancellationToken)
    {
        try
        {
            Directory.CreateDirectory("./temp");
            
            if (File.Exists("./temp/Database.db"))
                File.Delete("./temp/Database.db");

            // Crear conexión SQLite


            using var connection = new SqliteConnection(_sqliteConnectionString);
            await connection.OpenAsync(cancellationToken);

            foreach (var dbInfo in databaseInfos)
            {
                // Crear tabla
                var columnsDefinition = string.Join(", ", 
                    dbInfo.Columns.Select(col => $"{col.Name} {col.SqliteType}"));
                
                var createTableCommand = new SqliteCommand(
                    $"CREATE TABLE {dbInfo.TableName} ({columnsDefinition});", 
                    connection
                );
                await createTableCommand.ExecuteNonQueryAsync(cancellationToken);

                // Insertar datos
                foreach (var row in dbInfo.Data)
                {
                    var insertCommand = new SqliteCommand(
                        $"INSERT INTO {dbInfo.TableName} VALUES ({string.Join(", ", row)});", 
                        connection
                    );
                    await insertCommand.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            // Limpiar recursos
            GC.Collect();
            GC.WaitForPendingFinalizers();
            SqliteConnection.ClearAllPools();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
