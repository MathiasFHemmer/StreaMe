namespace api.Core.UnitOfWork;

public static class AmbientUnitOfWorkLocator
{
    private static readonly AsyncLocal<UnitOfWork?> _current = new();

    public static bool Get(out UnitOfWork context)
    {
        if (_current.Value is not null)
            return (context = _current.Value) != null;

        context = null;
        return false;    
    }

    internal static void SetCurrent(UnitOfWork uow) => _current.Value = uow;
    internal static void ClearCurrent() => _current.Value = null;
}
