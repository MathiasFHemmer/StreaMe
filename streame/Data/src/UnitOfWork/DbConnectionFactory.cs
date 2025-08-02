using System.Data.Common;

namespace Streame.Data.UnitOfWork;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}