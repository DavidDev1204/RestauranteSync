namespace RestauranteSync.Domain.Entities;

public class SyncResult
{
    public bool IsSuccess { get; private set; }
    public byte[]? Data { get; private set; }
    public string? ErrorMessage { get; private set; }

    private SyncResult(bool isSuccess, byte[]? data = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static SyncResult Success(byte[] data) => new(true, data);
    public static SyncResult Failure(string errorMessage) => new(false, errorMessage: errorMessage);
}
