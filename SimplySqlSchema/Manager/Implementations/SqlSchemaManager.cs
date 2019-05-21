using System;
using System.Collections.Generic;
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
            var statements = new List<string>();
            statements.AddRange(objectSchema.Columns.Values.Select(c => CreateColumnString(objectSchema.Name, c)));
            statements.Add(this.CreatePkStatement(objectSchema));

            var allStatements = string.Join(
                "," + Environment.NewLine,
                statements.Where(s => !string.IsNullOrEmpty(s))
            );

            await connection.ExecuteAsync($@"
                CREATE TABLE {objectSchema.Name} (
                    {allStatements}
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
                ALTER TABLE {objectName} ADD {CreateColumnString(objectName, columnSchema)}
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

        protected virtual string CreateColumnString(string objectName, ColumnSchema schema)
        {
            if (schema.SqlType == null)
            {
                throw new ArgumentNullException($"SqlType of column {schema.Name} cannot be null");
            }

            return string.Join(" ", new[]
            {
                CreateNameString(schema),
                CreateTypeString(schema) + CreateSizeAppendString(schema),
                CreateNullableString(schema),
                CreateDefaultValueString(objectName, schema),
            });
        }

        protected virtual string CreatePkStatement(ObjectSchema schema)
        {
            var primaryKeys = schema.Columns.Where(c => c.Value.KeyIndex.HasValue).OrderBy(c => c.Value.KeyIndex);
            if (!primaryKeys.Any())
            {
                return string.Empty;
            }

            return $"CONSTRAINT PK_{schema.Name} PRIMARY KEY ({string.Join(",", primaryKeys.Select(pk => pk.Key))})";
        }

        protected virtual string CreateNameString(ColumnSchema schema) => schema.Name;

        protected virtual string CreateTypeString(ColumnSchema schema) => schema.SqlType.ToString().ToUpper();

        protected virtual string CreateNullableString(ColumnSchema schema) => schema.Nullable ? "NULL" : "NOT NULL";

        protected virtual string CreateSizeAppendString(ColumnSchema schema) => schema.MaxLength.HasValue ? $"({schema.MaxLength})" : "";

        protected virtual string CreateDefaultValueString(string objectName, ColumnSchema schema)
        {
            return schema.Nullable ?
                "DEFAULT NULL" :
                $"DEFAULT {this.Mapper.GetDefaultValue(schema.SqlType)}";
        }

        protected virtual ColumnSchema ParseSqlTypeString(string typeDef)
        {
            var column = new ColumnSchema();
            var typePortions = typeDef.Split('(');
            SqlDbType sqlDbType;
            bool validType = Enum.TryParse<SqlDbType>(typePortions[0], ignoreCase: true, result: out sqlDbType);
            if (!validType)
            {
                throw new InvalidOperationException($"Column type {typeDef} could not be parsed to a SqlDbType");
            }
            column.SqlType = sqlDbType;

            if (typePortions.Length > 1 && this.Mapper.IsLengthLimited(column.SqlType))
            {
                column.MaxLength = Convert.ToInt32(typePortions[1].Trim(')'));
            }
            return column;
        }

        public SqlDbType MapColumnType(Type dotnetType)
        {
            return this.Mapper.GetSqlType(dotnetType);
        }
    }
}
