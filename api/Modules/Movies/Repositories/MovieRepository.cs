using api.Core.Entities;
using api.Core.Result;
using api.Core.UnitOfWork;
using Dapper;

namespace api.Modules.Movies.Repository;

public interface IMovieRepository
{
    Task<Result<Movie>> InsertNew(string title, string description, int releaseYear, string path, string hangfireJobId, CancellationToken ct);
}

public sealed class MovieRepository : IMovieRepository
{
    static class Errors
    {
        public enum Code
        {
            Generic
        }
        public static ErrorResult Generic(object data)
            => ErrorResult.New(Code.Generic, "Something went wrong! Data: {0}", data);
    }
    private readonly ILogger<MovieRepository> logger;

    public MovieRepository(ILogger<MovieRepository> logger)
    {
        this.logger = logger;
    }

    public async Task<Result<Movie>> InsertNew(string title, string description, int releaseYear, string path, string hangfireJobId, CancellationToken ct)
    {
        var movie = new Movie(title, releaseYear, description);
        var movieMetadata = new MovieMetadata(movie.Id, hangfireJobId)
        {
            LengthMinutes = 1,
            FileName = title, 
            FileLocation = path 
        };

        try
        {
            if (AmbientUnitOfWorkLocator.Get(out var uow))
            {
                var insertMovie = new CommandDefinition(
                    commandText: "INSERT INTO movies (id, title, release_year, description) VALUES (@Id, @Title, @ReleaseYear, @Description)",
                    parameters: movie,
                    transaction: uow.Transaction,
                    cancellationToken: ct
                );

                var insertMovieMetadata = new CommandDefinition(
                    commandText: "INSERT INTO movie_metadata (id, movie_id, status, length_minutes, file_name, file_location, hangfire_job_id) VALUES (@Id, @MovieId, @Status, @LengthMinutes, @FileName, @FileLocation, @HangfireJobId)",
                    parameters: movieMetadata,
                    transaction: uow.Transaction,
                    cancellationToken: ct
                );
                await uow.Connection.ExecuteAsync(insertMovie);
                await uow.Connection.ExecuteAsync(insertMovieMetadata);
            }
            logger.LogInformation("New Movie {id} added successfully", movie.Id);
            return Result.Success(movie);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to insert new Movie on database");
            return Result<Movie>.Failure(Errors.Generic(e));
        }
    }
}