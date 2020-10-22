using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SimplySqlSchema.Manager;

namespace SimplySqlSchema.SqlServer
{
    public class SqlServerSchemaManager : SqlSchemaManager
    {
        public override BackendType Backend => BackendType.SqlServer;

        public override async Task<ObjectSchema> GetSchema(IDbConnection connection, string objectName)
        {
            var columns = await connection.QueryAsync<SqlServerSchemaRow>($@"
                SELECT
  	                cols.*,
	                keys.ORDINAL_POSITION as KEY_POSITION
                FROM INFORMATION_SCHEMA.COLUMNS cols
                LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE keys 
	                ON cols.TABLE_NAME = keys.TABLE_NAME AND cols.COLUMN_NAME = keys.COLUMN_NAME
                WHERE cols.TABLE_NAME = '{objectName}';
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

        public SqlServerSchemaManager()
        {
            this.Mapper
                .Override(SqlDbType.Date, "'1753-1-1'");
        }

        private ColumnSchema ParseColumn(SqlServerSchemaRow row)
        {
            var column = new ColumnSchema()
            {
                Name = row.COLUMN_NAME,
                KeyIndex = row.KEY_POSITION,
                Nullable = row.IS_NULLABLE == "YES",
                MaxLength = row.CHARACTER_MAXIMUM_LENGTH,
            };
            SqlDbType sqlDbType;
            bool validType = Enum.TryParse<SqlDbType>(row.DATA_TYPE, ignoreCase: true, result: out sqlDbType);
            if (!validType)
            {
                throw new InvalidOperationException($"Column type {row.DATA_TYPE} could not be parsed to a SqlDbType");
            }
            column.SqlType = sqlDbType;
            return column;
        }

        public override async Task DeleteColumn(IDbConnection connection, string objectName, string columnName)
        {
            await connection.ExecuteAsync($@"
                ALTER TABLE {objectName} DROP CONSTRAINT {CreateConstraintName(objectName, columnName)}
            ");

            await base.DeleteColumn(connection, objectName, columnName);
        }

        protected override string CreateDefaultValueString(string objectName, ColumnSchema schema)
        {
            if (schema.SqlType == SqlDbType.Timestamp)
            {
                return string.Empty;
            }

            return $"CONSTRAINT {CreateConstraintName(objectName, schema.Name)} {base.CreateDefaultValueString(objectName, schema)}";
        }

        protected string CreateConstraintName(string objectName, string columnName)
        {
            return $"defaultval_{objectName}_{columnName}";
        }

        class SqlServerSchemaRow
        {
            public string COLUMN_NAME { get; set; }

            public string IS_NULLABLE { get; set; }

            public string DATA_TYPE { get; set; }

            public int? KEY_POSITION { get; set; }

            public int? CHARACTER_MAXIMUM_LENGTH { get; set; }
        }
    }
}
