using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Query;

namespace SimplySqlSchema
{
    public static class QueryExtensions
    {
        public static IServiceCollection AddAllQueriers(this IServiceCollection services)
        {
            return services
                .AddDefaultQuerier()
                .AddSQLiteQuerier()
                .AddMySqlQuerier()
                .AddSqlServerQuerier();
        }

        public static IServiceCollection AddDefaultQuerier(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaQuerier, SchemaQuerier>();
        }

        public static IServiceCollection AddSQLiteQuerier(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaQuerier, SQLiteQuerier>();
        }

        public static IServiceCollection AddMySqlQuerier(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaQuerier, MySqlQuerier>();
        }

        public static IServiceCollection AddSqlServerQuerier(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaQuerier, SqlServerQuerier>();
        }
    }
}
