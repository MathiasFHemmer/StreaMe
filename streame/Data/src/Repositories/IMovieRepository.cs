using Streame.Data.Models.Movie;
using Streame.Lib.Result;

namespace Streame.Data.Repositories;

public interface IMovieRepository
{
    Task<Result<Movie, Error>> InsertNew(string title, string description, int releaseYear, string path, string hangfireJobId, CancellationToken ct);
}
