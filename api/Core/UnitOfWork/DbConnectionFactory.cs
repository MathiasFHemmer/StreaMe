using System.Data.Common;

namespace api.Core.UnitOfWork;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}