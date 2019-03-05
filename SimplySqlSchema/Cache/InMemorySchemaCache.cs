using Microsoft.Extensions.Caching.Memory;

namespace SimplySqlSchema.Cache
{
    class InMemorySchemaCache : ISchemaCache
    {
        protected const string CacheKey = "Schemas";

        protected IMemoryCache MemoryCache { get; }

        public InMemorySchemaCache(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
        }

        public ObjectSchema Get(string objectName)
        {
            ObjectSchema result = null;
            this.MemoryCache.TryGetValue(objectName, out result);
            return result;
        }

        public void Set(ObjectSchema schema)
        {
            this.MemoryCache.Set(schema.Name, schema);
        }

        public void Remove(string objectName)
        {
            this.MemoryCache.Remove(objectName);
        }
    }
}
