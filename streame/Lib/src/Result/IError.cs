namespace Streame.Lib.Result;
public interface IError
{
    public IError? CausedBy { get; init; } 
}
