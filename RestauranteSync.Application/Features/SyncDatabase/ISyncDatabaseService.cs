using RestauranteSync.Domain.Entities;

namespace RestauranteSync.Application.Features.SyncDatabase;

public interface ISyncDatabaseService
{
    Task<SyncResult> SyncDatabaseAsync(CancellationToken cancellationToken);
}
