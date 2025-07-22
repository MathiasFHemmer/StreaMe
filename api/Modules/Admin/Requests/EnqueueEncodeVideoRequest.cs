using System.Text.Json.Serialization;
using api.Core.Result;
using api.Modules.Admin;

namespace api.Modules.Admin.Requests;

public sealed record class EnqueueEncodeVideoRequest
{
    public string VideoPath { get; set; }

    public EnqueueEncodeVideoRequest(string videoPath)
    {
        VideoPath = videoPath;
    }

    public static EnqueueEncodeVideoRequest From(string Path) => new EnqueueEncodeVideoRequest(Path);
    public Result IsValid()
    {
        if (string.IsNullOrEmpty(VideoPath))
            return Result.Failure($"Missing {nameof(VideoPath)}!", Errors.Code.VideoPathEmpty);

        if (!Path.Exists(VideoPath))
            return Result.Failure("Path {0} not found!", Errors.Code.PathNotFound, VideoPath);

        return Result.Success();
    }
}