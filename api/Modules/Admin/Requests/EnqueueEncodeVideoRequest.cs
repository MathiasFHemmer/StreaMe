using System.Text.Json.Serialization;
using api.Core.Result;
using api.Modules.Admin;

namespace api.Modules.Admin.Requests;

public sealed record class EnqueueEncodeVideoRequest
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }

    public Result IsValid()
    {
        if (string.IsNullOrEmpty(Path))
            return Result.Failure($"Missing {nameof(Path)}!", Errors.Code.VideoPathEmpty);

        if (!System.IO.Path.Exists(Path))
            return Result.Failure("Path {0} not found!", Errors.Code.PathNotFound, Path);

        return Result.Success();
    }
}