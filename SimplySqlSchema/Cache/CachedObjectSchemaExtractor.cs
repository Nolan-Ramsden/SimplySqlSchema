using System;

namespace SimplySqlSchema.Cache
{
    class CachedObjectSchemaExtractor : IObjectSchemaExtractor
    {
        protected ISchemaCache SchemaCache { get; } 
        protected IObjectSchemaExtractor Impl { get; }

        public CachedObjectSchemaExtractor(ISchemaCache schemaCache, IObjectSchemaExtractor impl)
        {
            this.Impl = impl;
            this.SchemaCache = schemaCache;
        }

        public ObjectSchema GetObjectSchema(Type t, string objectName)
        {
            var schema = this.SchemaCache.Get(objectName);
            if (schema == null)
            {
                schema = this.Impl.GetObjectSchema(t, objectName);
            }
            return schema;
        }
    }
}
