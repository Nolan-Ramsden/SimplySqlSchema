﻿using System;

namespace SimplySqlSchema.Cache
{
    public class CachedObjectSchemaExtractor : IObjectSchemaExtractor
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
                if (schema != null)
                {
                    this.SchemaCache.Set(schema);
                }
            }
            return schema;
        }
    }
}
