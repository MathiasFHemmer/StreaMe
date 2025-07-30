using api.Core.Repository;
using api.Core.UnitOfWork;
using api.Modules.MovieManager;
using api.Modules.MovieManager.Services;
using api.Modules.Movies.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace api.Setup;

public static class ApiSetup
{
    public static void SetupApiServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
            .AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>()
            .AddTransient<IMovieManagerService, MovieManagerService>()
            .AddTransient<IMovieRepository, MovieRepository>()
            .AddTransient<VideoEncodingJob>()
            .AddTransient<JobStateTracker>();
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