using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.SQLite;
using SimplySqlSchema.SqlServer;

namespace SimplySqlSchema
{
    public static class SqlServerExtensions
    {
        public static IServiceCollection AddSqlServerSupport(this IServiceCollection services)
        {
            return services
                .AddScoped<IConnectionFactory, SqlServerConnectionFactory>()
                .AddScoped<ISchemaManager, SqlServerSchemaManager>()
                .AddScoped<ISchemaQuerier, SqlServerQuerier>();
        }
    }
}