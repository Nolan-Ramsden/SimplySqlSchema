namespace SimplySqlSchema
{
    public interface ISchemaManagerDelegator
    {
        ISchemaManager GetSchemaManager(BackendType backendType, bool cacheWrap = false);

        ISchemaQuerier GetSchemaQuerier(BackendType backendType);

        IObjectSchemaExtractor GetSchemaExtractor(bool cacheWrap = false);
    }
}
