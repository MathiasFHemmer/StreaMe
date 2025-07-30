using api.Core.Entities;
using api.Core.Result;

namespace api.Modules.Movies.Repository;

public interface IMovieRepository
{
    Task<Result<Movie, Error>> InsertNew(string title, string description, int releaseYear, string path, string hangfireJobId, CancellationToken ct);
}
