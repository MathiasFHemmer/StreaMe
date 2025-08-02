using api.Modules.MovieManager.Requests;
using Hangfire;
using Streame.App.Errors;
using Streame.Data.UnitOfWork;
using Streame.Lib.Result;
using Microsoft.Extensions.Logging;
using Streame.Data.Repositories;

namespace api.Modules.MovieManager.Services;

public sealed class MovieManagerService(
    ILogger<MovieManagerService> logger,
    Configuration configuration,
    IUnitOfWorkFactory unitOfWorkFactory,
    IMovieRepository movieRepository,
    IBackgroundJobClient backgroundJobClient,
    VideoEncodingJob videoEncodingJob
) : IMovieManagerService
{
    public async Task<Result<Guid, IError>> AddMovie(AddMovieRequest request, CancellationToken ct)
    {
        if (request.IsInvalid(out var errors))
            return Result<Guid, IError>.Failure(new InvalidAPIRequest(errors));

        try
        {
            using (unitOfWorkFactory.Create())
            {
                logger.LogInformation("Adding movie: {Title}", request.Title);
                var inputPath = Path.Combine(configuration.MoviesInputPath, request.Path);
                var outputPath = configuration.MovieStoragePath;
                logger.LogInformation("Input path: {inputPath} | Output path: {outputPath}", inputPath, outputPath);
                var jobId = backgroundJobClient.Enqueue(() => videoEncodingJob.Run(inputPath, outputPath, request.Title));
                logger.LogInformation("Video encoding job enqueued with ID: {JobId}", jobId);
                var movie = await movieRepository.InsertNew(request.Title, request.Description, request.ReleaseYear, outputPath, jobId, ct);

                if (movie.IsFailure(out var insertNewError))
                    return Result<Guid, IError>.Failure(new Error("AddMovie", "Failed to insert new movie into the repository", insertNewError));

                logger.LogInformation("Movie {Id} added successfully with Hangfire Job ID: {JobId}", movie.Value!.Id, jobId);
                return Result<Guid, IError>.Success(movie.Value!.Id);
            }
        }
        catch (Exception ex)
        {
            return Result<Guid, IError>.Failure(new UnhandledException("Unable to add video for encoding!", ex, null));
        }
    }
}