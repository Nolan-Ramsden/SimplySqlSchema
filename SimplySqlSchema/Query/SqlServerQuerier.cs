using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace SimplySqlSchema.Query
{
    public class SqlServerQuerier : SchemaQuerier
    {
        public async override Task Update<T>(IDbConnection connection, ObjectSchema schema, T obj)
        {
            var names = schema.Columns.Select(c => c.Key);
            var variables = schema.Columns.Select(c => $"@{c.Key}");
            var updates = schema.Columns.Select(c => $"{c.Value.Name} = @{c.Value.Name}");
            var keys = schema.Columns.Values.Where(c => c.KeyIndex > 0).Select(c => c.Name);

            await connection.ExecuteAsync($@"
            BEGIN TRANSACTION
                IF EXISTS (
                    SELECT * 
                    FROM {schema.Name}
                    WHERE ({this.GetKeyFilter(schema)})
                )
                    BEGIN
                        UPDATE {schema.Name}
                        SET {string.Join(", ", updates)}
                        WHERE ({this.GetKeyFilter(schema)})
                    END
                ELSE
                    BEGIN
                        INSERT INTO {schema.Name}
                        ({string.Join(", ", names)})
                        VALUES
                        ({string.Join(", ", variables)})
                    END
            COMMIT TRANSACTION
            ", obj);
        }
    }
}
