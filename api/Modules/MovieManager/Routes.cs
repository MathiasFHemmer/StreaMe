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

    private static async Task<IResult> AddMovieHandler([FromBody] AddMovieRequest request, [FromServices] ILogger<Program> logger, [FromServices] IMovieManagerService service, CancellationToken ct)
    {
        var result = await service.AddMovie(request, ct);
        if (result.IsFailure(out var addMovieError))
        {
            logger.LogError("Failed to add movie: {Error}", addMovieError);
            if (addMovieError is InvalidAPIRequest invalidRequest)
            {
                return Results.BadRequest(new
                {
                    Message = "Invalid request",
                    Errors = invalidRequest.InvalidProperties.Select(p => new { p.PropertyName, p.Message })
                });
            } else if (addMovieError is Error error)
            {
                return Results.BadRequest(new { Message = error.Message, Details = error });
            }
            return Results.BadRequest(addMovieError);
        }
        
        logger.LogInformation("Movie added successfully with ID: {MovieId}", result.Value);
        return Results.Ok(new { MovieId = result.Value });
    }
}