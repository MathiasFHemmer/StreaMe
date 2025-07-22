using api.Core.Result;

namespace api.Modules.Admin;
static class Errors
{
    public enum Code
    {
        VideoPathEmpty,
        PathNotFound,
        JobIdNotFound,
        Generic
    }
    public static ErrorResult VideoPathEmpty
        => ErrorResult.New(Code.VideoPathEmpty, "Missing path of video!");
    public static ErrorResult PathNotFound(string path)
        => ErrorResult.New(Code.PathNotFound, "Could not find {0}!", path);
    public static ErrorResult JobIdNotFound(string jobId)
        => ErrorResult.New(Code.PathNotFound, "Could not find job {0}!", jobId);
    public static ErrorResult Generic(object data)
        => ErrorResult.New(Code.Generic, "Something went wrong! Data: {0}", data);
}