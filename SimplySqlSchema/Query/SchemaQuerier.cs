using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace SimplySqlSchema.Query
{
    public class SchemaQuerier : ISchemaQuerier
    {
        public BackendType Backend => BackendType.Default;

        public async virtual Task<T> Get<T>(IDbConnection connection, ObjectSchema schema, T keyedObject)
        {
            return await connection.QuerySingleOrDefaultAsync<T>($@"
                SELECT * FROM {schema.Name} WHERE {this.GetKeyFilter(schema)}
            ", keyedObject);
        }

        public async virtual Task Insert<T>(IDbConnection connection, ObjectSchema schema, T obj)
        {
            var names = schema.Columns.Select(c => c.Key);
            var variables = schema.Columns.Select(c => $"@{c.Key}");

            await connection.ExecuteAsync($@"
                INSERT INTO {schema.Name}
                ({string.Join(",", names)})
                VALUES
                ({string.Join(",", variables)})
            ", obj);
        }

        public async virtual Task Update<T>(IDbConnection connection, ObjectSchema schema, T obj)
        {
            var names = schema.Columns.Select(c => c.Key);
            var variables = schema.Columns.Select(c => $"@{c.Key}");
            var updates = schema.Columns.Select(c => $"{c.Value.Name} = @{c.Value.Name}");

            await connection.ExecuteAsync($@"
                INSERT INTO {schema.Name}
                ({string.Join(",", names)})
                VALUES
                ({string.Join(",", variables)})
                ON DUPLICATE KEY UPDATE
                {string.Join(",", updates)};
            ", obj);
        }

        public async virtual Task Delete<T>(IDbConnection connection, ObjectSchema schema, T keyedObject)
        {
            await connection.QuerySingleOrDefaultAsync<T>($@"
                DELETE FROM {schema.Name} WHERE {this.GetKeyFilter(schema)}
            ", keyedObject);
        }

        protected string GetKeyNames(ObjectSchema schema)
        {
            return string.Join(", ", schema.Columns.Values.Where(c => c.KeyIndex > 0).Select(c => c.Name));
        }

        protected string GetKeyFilter(ObjectSchema schema)
        {
            List<string> queries = new List<string>();
            foreach (var key in schema.Columns.Where(c => c.Value.KeyIndex > 0))
            {
                queries.Add($"{key.Value.Name} = @{key.Value.Name}");
            }
            return $"{string.Join(" AND ", queries)}";
        }
    }
}
