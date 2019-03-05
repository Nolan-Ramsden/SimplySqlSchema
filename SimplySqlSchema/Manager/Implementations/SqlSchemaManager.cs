using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace SimplySqlSchema.Manager.Implementations
{
    public class SqlSchemaManager : ISchemaManager
    {
        public virtual BackendType Backend => BackendType.Default;

        protected TypeMapper Mapper { get; } = new TypeMapper();

        public virtual Task<ObjectSchema> GetSchema(IDbConnection connection, string objectName)
        {
            throw new NotImplementedException("GetSchema is not implemented on the default provider");
        }

        public virtual async Task CreateObject(IDbConnection connection, ObjectSchema objectSchema)
        {
            var columnDefs = string.Join(
                "," + Environment.NewLine,
                objectSchema.Columns.Values.Select(CreateColumnString)
            );

            await connection.ExecuteAsync($@"
                CREATE TABLE {objectSchema.Name} (
                    {columnDefs}
                )
            ");
        }

        public virtual async Task DeleteObject(IDbConnection connection, string objectName)
        {
            await connection.ExecuteAsync($@"
                DROP TABLE IF EXISTS {objectName};
            ");
        }

        public virtual async Task CreateColumn(IDbConnection connection, string objectName, ColumnSchema columnSchema)
        {
            await connection.ExecuteAsync($@"
                ALTER TABLE {objectName} ADD {CreateColumnString(columnSchema)}
            ");
        }

        public virtual async Task DeleteColumn(IDbConnection connection, string objectName, string columnName)
        {
            await connection.ExecuteAsync($@"
                ALTER TABLE {objectName} DROP COLUMN {columnName}
            ");
        }

        public virtual Task CreateIndex(IDbConnection connection, string objectName, IndexSchema indexSchema)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteIndex(IDbConnection connection, string objectName, string indexName)
        {
            throw new NotImplementedException();
        }

        protected virtual string CreateColumnString(ColumnSchema schema)
        {
            return string.Join(" ", new[]
            {
                CreateNameString(schema),
                CreateTypeString(schema) + CreateSizeAppendString(schema),
                CreateNullableString(schema),
                CreateKeyString(schema),
                CreateDefaultValueString(schema),
            });
        }

        protected virtual string CreateNameString(ColumnSchema schema) => schema.Name;

        protected virtual string CreateTypeString(ColumnSchema schema) => this.Mapper.GetSqlType(schema.Type);

        protected virtual string CreateNullableString(ColumnSchema schema) => schema.Nullable ? "NULL" : "NOT NULL";

        protected virtual string CreateKeyString(ColumnSchema schema) => schema.KeyIndex.HasValue ? "PRIMARY KEY" : "";

        protected virtual string CreateSizeAppendString(ColumnSchema schema) => schema.MaxLength.HasValue ? $"({schema.MaxLength})" : "";

        protected virtual string CreateDefaultValueString(ColumnSchema schema)
        {
            return schema.Nullable ?
                "" :
                $"DEFAULT {this.Mapper.GetDefaultExpression(CreateTypeString(schema))}";
        }
    }
}
