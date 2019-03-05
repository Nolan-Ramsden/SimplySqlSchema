namespace SimplySqlSchema
{
    public interface ISchemaCache
    {
        ObjectSchema Get(string objectName);

        void Set(ObjectSchema schema);

        void Remove(string objectName);
    }
}
