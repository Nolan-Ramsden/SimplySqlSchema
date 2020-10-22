using System.Data;
using MySql.Data.MySqlClient;

namespace SimplySqlSchema.MySql
{
    public class MySqlConnectionFactory : IConnectionFactory
    {
        public BackendType Backend => BackendType.MySql;

        public IDbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
