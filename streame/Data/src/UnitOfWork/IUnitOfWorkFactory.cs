namespace Streame.Data.UnitOfWork;
/// <summary>
/// Use this class to create units of work. Each unit of work is bound to a transaction, and commits it once disposed.
/// </summary>
public interface IUnitOfWorkFactory
{
    UnitOfWork Create();
}