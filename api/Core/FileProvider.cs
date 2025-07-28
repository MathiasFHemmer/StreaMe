namespace api.Core;

public class FileProvider
{
    private static string MovieFolderConfigKey = "MoviesFolder";
    private readonly IConfiguration configuration;
    private readonly ILogger<FileProvider> logger;

    public FileProvider(IConfiguration configuration, ILogger<FileProvider> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task 
}