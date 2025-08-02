using api.Modules.MovieManager.Requests;
using Streame.Lib.Result;

namespace api.Modules.MovieManager.Services;

public interface IMovieManagerService
{
    Task<Result<Guid, IError>> AddMovie(AddMovieRequest request, CancellationToken ct);
}
