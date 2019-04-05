using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;

namespace SimplySqlSchema.Manager.Implementations
{
    public class SQLiteSchemaManager : SqlSchemaManager
    {
        public override BackendType Backend => BackendType.SQLite;

        public override async Task<ObjectSchema> GetSchema(IDbConnection connection, string objectName)
        {
            var columns = await connection.QueryAsync<SqlLiteSchemaRow>($@"
                SELECT * FROM pragma_table_info('{objectName}')
            ");

            if (!columns.Any())
            {
                return null;
            }

            return new ObjectSchema()
            {
                Name = objectName,
                Indexes = new Dictionary<string, IndexSchema>(),
                Columns = columns.Select(ParseColumn).ToDictionary(c => c.Name)
            };
        }

        public override Task DeleteColumn(IDbConnection connection, string objectName, string columnName)
        {
            throw new NotImplementedException("SQLite does not implement drop column");
        }

        private ColumnSchema ParseColumn(SqlLiteSchemaRow row)
        {
            var column = this.ParseSqlTypeString(row.type);
            column.Name = row.name;
            column.KeyIndex = row.pk == 0 ? null : new Nullable<int>(row.pk);
            column.Nullable = row.notnull == 0;
            return column;
        }

        class SqlLiteSchemaRow
        {
            public string name { get; set; }

            public int notnull { get; set; }

            public int pk { get; set; }

            public string type { get; set; }
        }
    }
}
