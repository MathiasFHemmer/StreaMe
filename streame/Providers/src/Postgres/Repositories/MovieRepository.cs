using Dapper;
using Microsoft.Extensions.Logging;
using Streame.Data.Models.Movie;
using Streame.Data.Repositories;
using Streame.Data.UnitOfWork;
using Streame.Lib.Result;

namespace Streame.Providers.Postgres.Repositories;

public sealed class MovieRepository : IMovieRepository
{
    private readonly ILogger<MovieRepository> logger;

    public MovieRepository(ILogger<MovieRepository> logger)
    {
        this.logger = logger;
    }

    public async Task<Result<Movie, Error>> InsertNew(string title, string description, int releaseYear, string path, string hangfireJobId, CancellationToken ct)
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
            logger.LogInformation("New Movie {id} added successfully. Movie Metadata added with it: {metaId}", movie.Id, movieMetadata.Id);
            return Result<Movie, Error>.Success(movie);
        }
        catch (Exception e)
        {
            logger.LogError(e, "MovieRepository.InsertNew");
            return Result<Movie, Error>.Failure(new Error("MovieRepository.InsertNew", "An exception occurred while inserting the movie into the database", null));
        }
    }

    Task<Result<Movie, Error>> IMovieRepository.InsertNew(string title, string description, int releaseYear, string path, string hangfireJobId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}