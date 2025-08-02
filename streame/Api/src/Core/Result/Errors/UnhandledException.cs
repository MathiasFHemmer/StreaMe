using Streame.Lib.Result;

namespace api.Core.Result;

public readonly record struct UnhandledException(string Message, Exception ex, IError? CausedBy) : IError
{
    public override string ToString() => $"{Message} {(CausedBy is not null ? $"(Caused by: {CausedBy})" : string.Empty)}";
}