using System.Data;
using System.Data.SQLite;

namespace SimplySqlSchema.SQLite
{
    public class SQLiteConnectionFactory : IConnectionFactory
    {
        public BackendType Backend => BackendType.SQLite;

        public IDbConnection GetConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
