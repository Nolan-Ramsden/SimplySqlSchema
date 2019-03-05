using System.Data;
using System.Threading.Tasks;
using SimplySqlSchema.Migration;

namespace SimplySqlSchema
{
    public interface ISchemaMigrator
    {
        Task<SchemaMigrationPlan> PlanMigration(IDbConnection connection, ISchemaManager targetManager, ObjectSchema targetSchema, MigrationOptions options);

        Task<SchemaMigrationPlan> Execute(IDbConnection connection, ISchemaManager targetManager, SchemaMigrationPlan plan);
    }
}
