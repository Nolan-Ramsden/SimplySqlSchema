using System;

namespace SimplySqlSchema
{
    public interface IObjectSchemaExtractor
    {
        ObjectSchema GetObjectSchema(Type t, string objectName);
    }
}
