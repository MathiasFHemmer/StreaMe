using Streame.Data;

namespace Providers;

public class LocalFileProvider : IVirtualFileProvider
{
    private readonly string _basePath;

    public LocalFileProvider(Configuration configuration)
    {
        _basePath = configuration.MovieStoragePath ?? throw new ArgumentNullException(nameof(configuration.MovieStoragePath), "Movie storage path must be configured.");
    }

    public async Task UploadAsync(Stream fileStream, string fileName, CancellationToken ct)
    {
        var fullPath = Path.Combine(_basePath, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);

        using (var fileStreamToWrite = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(fileStreamToWrite, ct);
        }
    }
}