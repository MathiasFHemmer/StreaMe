using api.Modules.MovieManager.Services;
using Api.Routes;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Providers;
using Streame.Data;
using Streame.Data.UnitOfWork;

namespace api.Setup;

public static class ApiSetup
{
    public static void SetupApiServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>()
            .AddTransient<IVirtualFileProvider, LocalFileProvider>()
            .AddTransient<IMovieManagerService, MovieManagerService>()
            .AddTransient<VideoEncodingJob>()
            .AddTransient<JobStateTracker>();
        builder.AddPostgresProvider();
    }

    public static WebApplication SetupApiApp(this WebApplication app)
    {
        var config = app.Services.GetRequiredService<Configuration>();
        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(config.MovieStoragePath),
                RequestPath = "/hls",
                ServeUnknownFileTypes = true,
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings =
                    {
                        [".m3u8"] = "application/vnd.apple.mpegurl",
                        [".ts"] = "video/mp2t"
                    }
                }
            });
            
        app.MapMovieManagerRoutes();
        return app;
    }
}