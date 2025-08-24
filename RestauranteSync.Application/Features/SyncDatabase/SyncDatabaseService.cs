using RestauranteSync.Domain.Entities;
using RestauranteSync.Domain.Repositories;
using System.IO.Compression;

namespace RestauranteSync.Application.Features.SyncDatabase;

public class SyncDatabaseService : ISyncDatabaseService
{
    private readonly ISyncRepository _repository;
    private readonly string _sqliteFilePath = "./temp/Database.db";

    public SyncDatabaseService(ISyncRepository repository)
    {
        _repository = repository;
    }

    public async Task<SyncResult> SyncDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Limpiar archivo existente si existe
            if (File.Exists(_sqliteFilePath))
                File.Delete(_sqliteFilePath);

            // Obtener información de la base de datos PostgreSQL
            var tableNames = await _repository.GetTableNamesAsync(cancellationToken);
            var databaseInfos = new List<DatabaseInfo>();

            foreach (var tableName in tableNames)
            {
                var columns = await _repository.GetColumnsByTableNameAsync(tableName, cancellationToken);
                var data = await _repository.GetDataByTableNameAsync(tableName, cancellationToken);

                databaseInfos.Add(new DatabaseInfo
                {
                    TableName = tableName,
                    Columns = columns,
                    Data = data
                });
            }

            // Crear base de datos SQLite
            var success = await _repository.CreateSqliteDatabaseAsync(databaseInfos, cancellationToken);
            if (!success)
                return SyncResult.Failure("Error al crear la base de datos SQLite");

            if (!File.Exists(_sqliteFilePath))
                return SyncResult.Failure("Base de datos no encontrada");

            // Crear archivo ZIP
            var zipBytes = await CreateZipFileAsync(_sqliteFilePath);
            return SyncResult.Success(zipBytes);
        }
        catch (Exception ex)
        {
            return SyncResult.Failure($"Error durante la sincronización: {ex.Message}");
        }
    }

    private static async Task<byte[]> CreateZipFileAsync(string filePath)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
        }

        memoryStream.Position = 0;

        await Task.CompletedTask;

        return memoryStream.ToArray();
    }
}
