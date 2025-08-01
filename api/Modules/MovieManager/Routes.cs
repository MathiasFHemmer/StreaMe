using api.Core.Result;
using api.Modules.MovieManager.Requests;
using api.Modules.MovieManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Modules.MovieManager;

public static class MovieManagerRoutes
{
    public static void MapMovieManagerRoutes(this WebApplication app)
    {
        app.MapPost("/add", AddMovieHandler);
    }

    private static async Task<IResult> AddMovieHandler([FromBody] AddMovieRequest request, ILogger<Program> logger, [FromServices] IMovieManagerService service, CancellationToken ct)
    {
        var result = await service.AddMovie(request, ct);
        if (result.IsFailure(out var addMovieError))
            logger.LogError("Failed to add movie: {Error}", addMovieError);

        return result.ToAPIResult();
    }
}