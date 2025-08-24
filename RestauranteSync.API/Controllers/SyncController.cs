using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteSync.Application.Features.SyncDatabase;

namespace RestauranteSync.API.Controllers;

[ApiController]
[Route("api/sync")]
[Authorize] // Proteger todo el controlador
public class SyncController(ISyncDatabaseService syncService) : ControllerBase
{
    private readonly ISyncDatabaseService _syncService = syncService;

    [HttpGet("/database")]
    public async Task<IActionResult> GetDatabase(CancellationToken cancellationToken)
    {
        var result = await _syncService.SyncDatabaseAsync(cancellationToken);

        if (result.IsSuccess)
            return File(result.Data!, "application/zip", fileDownloadName: "Database.zip");

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }
}
