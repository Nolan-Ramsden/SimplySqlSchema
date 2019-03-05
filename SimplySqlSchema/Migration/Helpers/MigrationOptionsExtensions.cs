using System;

namespace SimplySqlSchema.Migration.Helpers
{
    public static class MigrationOptionsExtensions
    {
        public static void AssertColumnDeleteAllowed(this MigrationOptions opts, string table, string column)
        {
            opts.AssertNotFrozen($"delete '{column}' of '{table}'");
            if (!opts.AllowDataloss)
            {
                throw new InvalidOperationException($"Can't delete column '{column}' of '{table}', dataloss is not allowed");
            }
        }

        public static void AssertColumnDeleteAllowed(this MigrationOptions opts, string table, string column, string field)
        {
            opts.AssertNotFrozen($"change '{field}' of '{column}' of '{table}'");
            if (!opts.AllowDataloss)
            {
                throw new InvalidOperationException($"Can't migrate column '{column}' of '{table}', {field} can't be changed without dataloss");
            }
        }

        public static void AssertTableDeleteAllowed(this MigrationOptions opts, string table)
        {
            opts.AssertNotFrozen($"delete '{table}'");
            if (!opts.AllowDataloss)
            {
                throw new InvalidOperationException($"Can't delete table '{table}', dataloss is not allowed");
            }
        }


        public static void AssertNotFrozen(this MigrationOptions opts, string action)
        {
            if (opts.Freeze)
            {
                throw new InvalidOperationException($"Can't {action}, schema is frozen");
            }
        }
    }
}
