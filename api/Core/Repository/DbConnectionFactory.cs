using System.Data.Common;
using api.Core.UnitOfWork;
using Npgsql;

namespace api.Core.Repository;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    public IConfiguration configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(configuration.GetConnectionString("ApiDb"));
    }
}
