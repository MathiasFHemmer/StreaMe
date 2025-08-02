
using System.Data.Common;

namespace Streame.Data.UnitOfWork;

public sealed class UnitOfWork : IDisposable, IAsyncDisposable
{
    private readonly DbConnection dbConnection;
    private readonly DbTransaction dbTransaction;

    public DbConnection Connection => dbConnection;
    public DbTransaction Transaction => dbTransaction;

    private bool disposed;
    private bool isOuter;

    public UnitOfWork(IDbConnectionFactory dbConnectionFactory)
    {
        if (AmbientUnitOfWorkLocator.Get(out var uow))
        {
            dbConnection = uow.dbConnection;
            dbTransaction = uow.dbTransaction;
            isOuter = false;
        }
        else
        {
            dbConnection = dbConnectionFactory.CreateConnection();
            dbConnection.Open();
            dbTransaction = dbConnection.BeginTransaction();
            isOuter = true;
            AmbientUnitOfWorkLocator.SetCurrent(this);
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        if (!isOuter)
            return;

        try
        {
            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
        finally
        {
            dbTransaction.Dispose();
            dbConnection.Close();
            dbConnection.Dispose();
            AmbientUnitOfWorkLocator.ClearCurrent();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
            return;

        disposed = true;
        if (!isOuter)
            return;

        try
        {
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
        finally
        {
            await dbTransaction.DisposeAsync();
            await dbConnection.DisposeAsync();
            AmbientUnitOfWorkLocator.ClearCurrent();
        }
    }
}