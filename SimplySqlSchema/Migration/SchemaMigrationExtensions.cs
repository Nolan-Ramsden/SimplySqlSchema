using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Migration;

namespace SimplySqlSchema
{
    public static class SchemaMigrationExtensions
    {
        public static IServiceCollection AddSchemaMigrator(this IServiceCollection services)
        {
            return services.AddScoped<ISchemaMigrator, SchemaMigrator>();
        }

        public static async Task PlanAndMigrate(this ISchemaMigrator migrator, IDbConnection connection, ISchemaManager targetManager, ObjectSchema targetSchema, MigrationOptions options)
        {
            var plan = await migrator.PlanMigration(
                connection: connection,
                targetManager: targetManager,
                targetSchema: targetSchema,
                options: options
            );

            await migrator.ExecuteMigration(
                connection: connection,
                targetManager: targetManager,
                migrationPlan: plan
            );
        }
    }
}
