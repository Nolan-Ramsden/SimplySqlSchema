using Microsoft.Extensions.DependencyInjection;

namespace SimplySqlSchema
{
    public static class SimplySqlSchemaExtensions
    {
        public static IServiceCollection AddSimplySqlSchema(this IServiceCollection services)
        {
            return services
                .AddAllSchemaManagers()
                .AddInMemorySchemaCache()
                .AddSchemaManagerDelegator()
                .AddSchemaMigrator()
                .AddAllQueriers()
                .AddDataAnnotationExtractor();
        }
    }
}
