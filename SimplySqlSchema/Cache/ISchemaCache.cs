using System.Collections.Generic;

namespace SimplySqlSchema
{
    public interface ISchemaCache
    {
        IEnumerable<string> ListObjects();

        ObjectSchema Get(string objectName);

        void Set(ObjectSchema schema);

        void Remove(string objectName);
    }
}
