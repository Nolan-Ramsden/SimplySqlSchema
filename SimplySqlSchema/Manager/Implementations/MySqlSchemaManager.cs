using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace SimplySqlSchema.Manager.Implementations
{
    public class MySqlSchemaManager : SqlSchemaManager
    {
        public override BackendType Backend => BackendType.MySql;

        public override async Task<ObjectSchema> GetSchema(IDbConnection connection, string objectName)
        {
            IEnumerable<MySqlSchemaRow> columns;
            try
            {
                columns = await connection.QueryAsync<MySqlSchemaRow>($@"
                    DESCRIBE {objectName};
                ");
            }
            catch(Exception e) when (e.Message.Contains("doesn't exist"))
            {
                return null;
            }

            var keyIndex = 1;
            var typedColumns = new List<ColumnSchema>();
            foreach(var column in columns)
            {
                var typedColumn = ParseColumn(column, keyIndex); 
                if (typedColumn.KeyIndex > 0)
                {
                    keyIndex++;
                }
                typedColumns.Add(typedColumn);
            }

            return new ObjectSchema()
            {
                Name = objectName,
                Columns = typedColumns.ToDictionary(c => c.Name),
                Indexes = new Dictionary<string, IndexSchema>(),
            };
        }

        private ColumnSchema ParseColumn(MySqlSchemaRow row, int keyIndex)
        {
            var column = new ColumnSchema()
            {
                Name = row.Field,
                KeyIndex = row.Key != "PRI" ? null : (int?)keyIndex,
                Nullable = row.Null == "YES",
            };
            var typePortions = row.Type.Split(',')[0].Split('(');
            column.Type = this.Mapper.GetDotnetType(typePortions[0]);
            if (column.Type == null)
            {
                throw new InvalidOperationException($"Uncrecognized column type {typePortions[0]}");
            }
            if (typePortions.Length > 1 && column.Type == typeof(string))
            {
                column.MaxLength = Convert.ToInt32(typePortions[1].Trim(')'));
            }
            return column;
        }

        class MySqlSchemaRow
        {
            public string Field { get; set; }

            public string Null{ get; set; }

            public string Type { get; set; }

            public string Key { get; set; }
        }
    }
}
