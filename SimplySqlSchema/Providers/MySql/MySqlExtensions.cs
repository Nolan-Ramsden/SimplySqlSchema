using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.MySql;

namespace SimplySqlSchema
{
    public static class MySqlExtensions
    {
        public static IServiceCollection AddMySqlSupport(this IServiceCollection services)
        {
            return services
                .AddScoped<IConnectionFactory, MySqlConnectionFactory>()
                .AddScoped<ISchemaManager, MySqlSchemaManager>()
                .AddScoped<ISchemaQuerier, MySqlQuerier>();
        }
    }
}
