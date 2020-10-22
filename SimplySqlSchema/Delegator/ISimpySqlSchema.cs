namespace SimplySqlSchema
{
    public interface ISimplySqlSchema
    {
        ISchemaMigrator GetMigrator();
     
        IObjectSchemaExtractor GetExtractor();

        ISchemaManager GetManager(BackendType backendType);

        ISchemaQuerier GetQuerier(BackendType backendType);

        IConnectionFactory GetConnectionFactory(BackendType backendType);
    }
}
