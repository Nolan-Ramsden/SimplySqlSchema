using System.Data;
using System.Data.SqlClient;

namespace SimplySqlSchema.SQLite
{
    public class SqlServerConnectionFactory : IConnectionFactory
    {
        public BackendType Backend => BackendType.SqlServer;

        public IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
