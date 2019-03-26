using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace SimplySqlSchema.Query
{
    public class SQLiteQuerier : SchemaQuerier
    {
        public override BackendType Backend => BackendType.SQLite;

        public async override Task Update<T>(IDbConnection connection, ObjectSchema schema, T obj)
        {
            var names = schema.Columns.Select(c => c.Key);
            var variables = schema.Columns.Select(c => $"@{c.Key}");
            var updates = schema.Columns.Select(c => $"{c.Value.Name} = @{c.Value.Name}");
            var keys = schema.Columns.Values.Where(c => c.KeyIndex > 0).Select(c => c.Name);

            await connection.ExecuteAsync($@"
                INSERT INTO {schema.Name}
                ({string.Join(",", names)})
                VALUES
                ({string.Join(",", variables)})
                ON CONFLICT
                ({string.Join(", ", keys)})
                DO UPDATE SET
                {string.Join(",", updates)};
            ", obj);
        }
    }
}
