namespace SimplySqlSchema
{
    public interface ISchemaManagerDelegator
    {
        ISchemaManager GetSchemaManager(BackendType backendType);

        ISchemaQuerier GetSchemaQuerier(BackendType backendType);

        IObjectSchemaExtractor GetSchemaExtractor();
    }
}
