using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Cache;

namespace SimplySqlSchema
{
    public static class SchemaCacheExtensions
    {
        public static IServiceCollection AddInMemorySchemaCache(this IServiceCollection services)
        {
            return services
                .AddMemoryCache()
                .AddScoped<ISchemaCache, InMemorySchemaCache>();
        }

        public static IServiceCollection AddDictionarySchemaCache(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaCache, DictionarySchemaCache>();
        }
    }
}
