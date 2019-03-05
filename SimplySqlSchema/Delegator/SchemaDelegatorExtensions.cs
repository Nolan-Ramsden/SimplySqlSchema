using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Delegator;

namespace SimplySqlSchema
{
    public static class SchemaDelegatorExtensions
    {
        public static IServiceCollection AddSchemaManagerDelegator(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaManagerDelegator, SchemaManagerDelegator>();
        }
    }
}
