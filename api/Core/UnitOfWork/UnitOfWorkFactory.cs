namespace api.Core.UnitOfWork;

public sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private IDbConnectionFactory dbConnectionFactory;

    public UnitOfWorkFactory(IDbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public UnitOfWork Create()
    {
        return new UnitOfWork(dbConnectionFactory);
    }
}