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
            var column = new ColumnSchema()
            {
                Name = row.name,
                KeyIndex = row.pk == 0 ? null : new Nullable<int>(row.pk),
                Nullable = row.notnull == 0,
            };
            var typePortions = row.type.Split(',')[0].Split('(');
            column.Type = this.Mapper.GetDotnetType(typePortions[0]);
            if (column.Type == null)
            {
                throw new InvalidOperationException($"Uncrecognized column type {typePortions[0]}");
            }
            if (typePortions.Length > 1)
            {
                column.MaxLength = Convert.ToInt32(typePortions[1].Trim(')'));
            }
            
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
