using api.Core.Result;
using api.Modules.MovieManager.Requests;

namespace api.Modules.MovieManager.Services;

public interface IMovieManagerService
{
    Task<Result<Guid, IError>> AddMovie(AddMovieRequest request, CancellationToken ct);
}
