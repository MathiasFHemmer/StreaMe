using api.Core.Result;
using api.Core.UnitOfWork;
using Dapper;

namespace api.Modules.Movies.Repository;

public class Movie {
    public Guid Id { get; set; }
    public string Title { get; set; }
}

public class MovieMetadata
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public MovieStatus Status { get; set; }
}

public enum MovieStatus
{
    Ok,
    Processing,
    Error,
}

public interface IMovieRepository
{
    Task<Result<Movie>> InsertNew(string title, CancellationToken ct);
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

    public async Task<Result<Movie>> InsertNew(string title, CancellationToken ct)
    {
        var movie = new Movie
        {
            Id = Guid.CreateVersion7(),
            Title = title,
        };

        var movieMetadata = new MovieMetadata
        {
            Id = Guid.CreateVersion7(),
            MovieId = movie.Id,
            Status = MovieStatus.Processing
        };


        try
        {
            if (AmbientUnitOfWorkLocator.Get(out var uow))
            {
                var insertMovie = new CommandDefinition(
                    commandText: "INSERT INTO movies (id, title) VALUES (@Id, @Title)",
                    parameters: movie,
                    transaction: uow.Transaction,
                    cancellationToken: ct
                );

                var insertMovieMetadata = new CommandDefinition(
                    commandText: "INSERT INTO movie_metadata (id, movie_id, status) VALUES (@Id, @MovieId, @Status)",
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