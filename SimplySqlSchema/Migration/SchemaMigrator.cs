using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SimplySqlSchema.Migration.Helpers;

namespace SimplySqlSchema.Migration
{
    public class SchemaMigrator : ISchemaMigrator
    {
        public async Task<SchemaMigrationPlan> PlanMigration(IDbConnection connection, ISchemaManager targetManager, ObjectSchema targetSchema, MigrationOptions options)
        {
            options = options ?? new MigrationOptions();

            // Whichever columns are mapped to SqlType, we let the manager decide the best choice
            foreach (var column in targetSchema.Columns.Values)
            {
                if (column.SqlType == null)
                {
                    column.SqlType = targetManager.MapColumnType(column.DotnetType);
                }
            }

            var report = new SchemaMigrationPlan()
            {
                Target = targetSchema,
            };

            report.Existing = await targetManager.GetSchema(connection, targetSchema.Name);
            if (report.Existing == null)
            {
                report.Steps.Add(new MigrationStep()
                {
                    TargetName = targetSchema.Name,
                    Action = MigrationAction.Create,
                    TargetType = MigrationTarget.Object,
                });
                return report;
            }


            if (options.ForceDropRecreate)
            {
                options.AssertTableDeleteAllowed(targetSchema.Name);
                report.Steps.Add(new MigrationStep()
                {
                    TargetName = targetSchema.Name,
                    Action = MigrationAction.Drop,
                    TargetType = MigrationTarget.Object,
                });
                report.Steps.Add(new MigrationStep()
                {
                    TargetName = targetSchema.Name,
                    Action = MigrationAction.Create,
                    TargetType = MigrationTarget.Object,
                });
                return report;
            }

            var allColumnNames = report.Target.Columns.Keys.Union(report.Existing.Columns.Keys);
            foreach(var column in allColumnNames)
            {
                var targetColumn = report.Target.Columns.ContainsKey(column) ? report.Target.Columns[column] : null;
                var existingColumn = report.Existing.Columns.ContainsKey(column) ? report.Existing.Columns[column] : null;

                PlanMigrateColumn(report, existingColumn, targetColumn, options);
            }

            var allIndexNames = report.Target.Indexes.Keys.Union(report.Existing.Indexes.Keys);
            foreach (var index in allIndexNames)
            {
                var targetIndex = report.Target.Columns.ContainsKey(index) ? report.Target.Columns[index] : null;
                var existingIndex = report.Existing.Columns.ContainsKey(index) ? report.Existing.Columns[index] : null;

                PlanMigrateIndex(report, existingIndex, targetIndex, options);
            }

            return report;
        }

        public async Task<SchemaMigrationPlan> ExecuteMigration(IDbConnection connection, ISchemaManager targetManager, SchemaMigrationPlan plan)
        {
            foreach(var step in plan.Steps)
            {
                await ExecuteMigrationStep(connection, targetManager, step, plan.Target);
            }
            return plan;
        }

        protected async Task ExecuteMigrationStep(IDbConnection connection, ISchemaManager targetManager, MigrationStep step, ObjectSchema targetSchema)
        {
            if (step.Action == MigrationAction.Alter)
            {
                throw new NotImplementedException($"Explicit alters are not supported");
            }

            if (step.TargetType == MigrationTarget.Object)
            {
                if (step.Action == MigrationAction.Create)
                {
                    await targetManager.CreateObject(connection, targetSchema);
                }
                else if (step.Action == MigrationAction.Drop)
                {
                    await targetManager.DeleteObject(connection, step.TargetName);
                }
            }
            else if (step.TargetType == MigrationTarget.Column)
            {
                if (step.Action == MigrationAction.Create)
                {
                    await targetManager.CreateColumn(connection, targetSchema.Name, targetSchema.Columns[step.TargetName]);
                }
                else if (step.Action == MigrationAction.Drop)
                {
                    await targetManager.DeleteColumn(connection, targetSchema.Name, step.TargetName);
                }
            }
            else if (step.TargetType == MigrationTarget.Index)
            {
                if (step.Action == MigrationAction.Create)
                {
                    await targetManager.CreateIndex(connection, targetSchema.Name, targetSchema.Indexes[step.TargetName]);
                }
                else if (step.Action == MigrationAction.Drop)
                {
                    await targetManager.DeleteIndex(connection, targetSchema.Name, step.TargetName);
                }
            }
            else
            {
                throw new NotImplementedException($"No implementation for migration step");
            }
        }

        protected void PlanMigrateIndex(SchemaMigrationPlan report, ColumnSchema existingIndex, ColumnSchema targetIndex, MigrationOptions options)
        {
            throw new NotImplementedException();
        }

        protected void PlanMigrateColumn(SchemaMigrationPlan report, ColumnSchema existing, ColumnSchema target, MigrationOptions options)
        {
            if (existing == null)
            {
                if (target.KeyIndex > 0)
                {
                    throw new InvalidOperationException($"Cannot migrate column {target.Name} of {report.Target.Name}, keys can't be changed");
                }
                options.AssertNotFrozen($"Create column {target.Name} of {report.Target.Name}");

                report.Steps.Add(new MigrationStep()
                {
                    TargetName = target.Name,
                    Action = MigrationAction.Create,
                    TargetType = MigrationTarget.Column,
                });
                return;
            }

            if (target == null)
            {
                if (existing.KeyIndex > 0)
                {
                    throw new InvalidOperationException($"Cannot migrate column {existing.Name} of {report.Target.Name}, keys can't be changed");
                }
                options.AssertColumnDeleteAllowed(report.Target.Name, existing.Name);

                report.Steps.Add(new MigrationStep()
                {
                    TargetName = existing.Name,
                    Action = MigrationAction.Drop,
                    TargetType = MigrationTarget.Column,
                });
                return;
            }

            bool mismatched = false;
            if (target.KeyIndex != existing.KeyIndex)
            {
                throw new InvalidOperationException($"Cannot migrate column {target.Name} of {report.Target.Name}, keys can't be changed");
            }

            if (target.SqlType != existing.SqlType)
            {
                mismatched = true;
                options.AssertColumnDeleteAllowed(report.Target.Name, target.Name, "type");
            }

            if (target.Nullable != existing.Nullable)
            {
                mismatched = true;
                options.AssertColumnDeleteAllowed(report.Target.Name, target.Name, "nullable");
            }

            if (target.MaxLength != existing.MaxLength)
            {
                mismatched = true;
                options.AssertColumnDeleteAllowed(report.Target.Name, target.Name, "maxlength");
            }

            if (mismatched)
            {
                options.AssertColumnDeleteAllowed(report.Target.Name, target.Name);
                report.Steps.Add(new MigrationStep()
                {
                    TargetName = target.Name,
                    Action = MigrationAction.Drop,
                    TargetType = MigrationTarget.Column,
                });
                report.Steps.Add(new MigrationStep()
                {
                    TargetName = target.Name,
                    Action = MigrationAction.Create,
                    TargetType = MigrationTarget.Column,
                });
                return;
            }
        }
    }
}
