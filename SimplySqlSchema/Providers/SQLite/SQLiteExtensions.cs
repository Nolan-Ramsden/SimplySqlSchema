using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.SQLite;

namespace SimplySqlSchema
{
    public static class SQLiteExtensions
    {
        public static IServiceCollection AddSQLiteSupport(this IServiceCollection services)
        {
            return services
                .AddScoped<IConnectionFactory, SQLiteConnectionFactory>()
                .AddScoped<ISchemaManager, SQLiteSchemaManager>()
                .AddScoped<ISchemaQuerier, SQLiteQuerier>();
        }
    }
}
