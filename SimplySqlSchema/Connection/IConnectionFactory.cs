using System.Data;

namespace SimplySqlSchema
{
    public interface IConnectionFactory
    {
        BackendType Backend { get; }

        IDbConnection GetConnection(string connectionString);
    }
}
