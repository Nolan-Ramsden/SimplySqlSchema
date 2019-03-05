using System.Collections.Generic;

namespace SimplySqlSchema.Cache
{
    public class DictionarySchemaCache : ISchemaCache
    {
        protected Dictionary<string, ObjectSchema> Cache { get; } = new Dictionary<string, ObjectSchema>();

        public ObjectSchema Get(string objectName)
        {
            if (Cache.ContainsKey(objectName))
            {
                return Cache[objectName];
            }
            return null;
        }

        public void Remove(string objectName)
        {
            Cache.Remove(objectName);
        }

        public void Set(ObjectSchema schema)
        {
            Cache[schema.Name] = schema;
        }
    }
}
