namespace Streame.Data;

public interface IVirtualFileProvider
{
    Task UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default);   
}
