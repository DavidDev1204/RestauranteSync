using RestauranteSync.Domain.Entities;

namespace RestauranteSync.Domain.Repositories;

public interface ISyncRepository
{
    Task<List<string>> GetTableNamesAsync(CancellationToken cancellationToken);
    Task<List<ColumnInfo>> GetColumnsByTableNameAsync(string tableName, CancellationToken cancellationToken);
    Task<List<List<string>>> GetDataByTableNameAsync(string tableName, CancellationToken cancellationToken);
    Task<bool> CreateSqliteDatabaseAsync(List<DatabaseInfo> databaseInfos, CancellationToken cancellationToken);
}
