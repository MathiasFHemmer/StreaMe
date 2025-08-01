namespace api.Core.Result;

public interface IError
{
    public IError? CausedBy { get; init; } 
}
