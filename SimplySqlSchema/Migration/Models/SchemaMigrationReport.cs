using System.Collections.Generic;

namespace SimplySqlSchema.Migration
{
    public class SchemaMigrationPlan
    {
        public ObjectSchema Existing { get; set; }

        public ObjectSchema Target { get; set; }

        public IList<MigrationStep> Steps { get; set; } = new List<MigrationStep>();
    }
}
