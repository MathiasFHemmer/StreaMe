using System.Text.Json;
using api.Core.Result;

public static class ResultMapper
{
    public static IResult ToAPIResult<TValue, TError>(this Result<TValue, TError> result) where TError : IError
    {
        if (result.IsSuccess(out var value))
            return Results.Ok(JsonSerializer.Serialize(value));

        return MapError(result.Error!);
    }

    public static IResult ToAPIResult<TError>(this Result<TError> result) where TError : IError
    {
        if (result.IsSuccess)
            return Results.Ok();
        return MapError(result.Error!);
    }

    private static IResult MapError<TError>(TError error) where TError : IError
    {
        switch (error)
        {
            case InvalidAPIRequest invalidRequest:
                return Results.BadRequest(JsonSerializer.Serialize(invalidRequest));
            case Error apiError:
                return Results.BadRequest(JsonSerializer.Serialize(apiError));
            case UnhandledException unhandledException:
                return Results.InternalServerError(JsonSerializer.Serialize(new
                {
                    unhandledException.Message,
                    unhandledException.CausedBy
                }));
            default:
                return Results.InternalServerError("Unhandled error occurred.");
        }
    }
}
