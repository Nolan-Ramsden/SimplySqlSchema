namespace SimplySqlSchema
{
    public interface ISchemaManagerDelegator
    {
        ISchemaManager GetManager(BackendType backendType);

        ISchemaQuerier GetQuerier(BackendType backendType);

        IObjectSchemaExtractor GetSchemaExtractor();
    }
}
