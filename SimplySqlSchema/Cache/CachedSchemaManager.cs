using System;
using System.Data;
using System.Threading.Tasks;

namespace SimplySqlSchema.Cache
{
    public class CachedSchemaManager : ISchemaManager
    {
        protected ISchemaManager Impl { get; }
        protected ISchemaCache SchemaCache { get; }

        public BackendType Backend => this.Impl.Backend;

        public CachedSchemaManager(ISchemaCache schemaCache, ISchemaManager impl)
        {
            this.Impl = impl;
            this.SchemaCache = schemaCache;
        }

        public async Task<ObjectSchema> GetSchema(IDbConnection connection, string objectName)
        {
            var schema = this.SchemaCache.Get(objectName);
            if (schema == null)
            {
                schema = await this.Impl.GetSchema(connection, objectName);
                if (schema != null)
                {
                   this.SchemaCache.Set(schema);
                }
            }
            return schema;
        }

        public async Task CreateObject(IDbConnection connection, ObjectSchema objectSchema)
        {
            await this.Impl.CreateObject(connection, objectSchema);
            this.SchemaCache.Set(objectSchema);
        }

        public async Task DeleteObject(IDbConnection connection, string objectName)
        {
            await this.Impl.DeleteObject(connection, objectName);
            this.SchemaCache.Remove(objectName);
        }

        public async Task CreateColumn(IDbConnection connection, string objectName, ColumnSchema columnSchema)
        {
            await this.Impl.CreateColumn(connection, objectName, columnSchema);
            var schema = this.SchemaCache.Get(objectName);
            if (schema != null)
            {
                schema.Columns[columnSchema.Name] = columnSchema;
            }
        }

        public async Task DeleteColumn(IDbConnection connection, string objectName, string columnName)
        {
            await this.Impl.DeleteColumn(connection, objectName, columnName);
            var schema = this.SchemaCache.Get(objectName);
            if (schema != null)
            {
                schema.Columns.Remove(columnName);
            }
        }

        public Task CreateIndex(IDbConnection connection, string objectName, IndexSchema indexSchema)
        {
            throw new NotImplementedException();
        }

        public Task DeleteIndex(IDbConnection connection, string objectName, string indexName)
        {
            throw new NotImplementedException();
        }

        public SqlDbType MapColumnType(Type dotnetType)
        {
            return this.Impl.MapColumnType(dotnetType);
        }
    }
}
