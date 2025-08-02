using api.Core.Result;
using api.Modules.MovieManager.Requests;
using api.Modules.MovieManager.Services;
using Microsoft.AspNetCore.Mvc;
using Streame.Data;

namespace Api.Routes;

public static class MovieManagerRoutes
{
    public static void MapMovieManagerRoutes(this WebApplication app)
    {
        app.MapPost("/add", AddMovieHandler);
        app.MapPost("/upload", UploadMovieHandler);
    }

    private static async Task<IResult> AddMovieHandler([FromBody] AddMovieRequest request, ILogger<Program> logger, [FromServices] IMovieManagerService service, CancellationToken ct)
    {
        var result = await service.AddMovie(request, ct);
        if (result.IsFailure(out var addMovieError))
            logger.LogError("Failed to add movie: {Error}", addMovieError);

        return result.ToAPIResult();
    }

    private static async Task<IResult> UploadMovieHandler(HttpRequest request, ILogger<Program> logger, [FromServices] IVirtualFileProvider fileProvider, CancellationToken ct)
    {
        var filenameHeader = request.Headers["X-Filename"].FirstOrDefault();
        logger.LogInformation("Received file upload request with filename: {FileName}", filenameHeader);
        if (string.IsNullOrWhiteSpace(filenameHeader))
            return Results.BadRequest("Missing X-Filename header");

        var filename = Uri.UnescapeDataString(filenameHeader);
        await fileProvider.UploadAsync(request.Body, filename, ct);
        logger.LogInformation("File {FileName} uploaded successfully", filename);

        return Results.Ok("Uploaded");
    }
}