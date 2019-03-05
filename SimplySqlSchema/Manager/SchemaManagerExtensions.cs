using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Manager.Implementations;

namespace SimplySqlSchema
{
    public static class SchemaManagerExtensions
    {
        public static IServiceCollection AddAllSchemaManagers(this IServiceCollection services)
        {
            return services
                .AddDefaultSchemaManager()
                .AddSQLiteSchemaManager()
                .AddSqlServerSchemaManager()
                .AddMySqlSchemaManager();
        }

        public static IServiceCollection AddDefaultSchemaManager(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaManager, SqlSchemaManager>();
        }

        public static IServiceCollection AddSQLiteSchemaManager(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaManager, SQLiteSchemaManager>();
        }

        public static IServiceCollection AddSqlServerSchemaManager(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaManager, SqlServerSchemaManager>();
        }

        public static IServiceCollection AddMySqlSchemaManager(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaManager, MySqlSchemaManager>();
        }
    }
}
