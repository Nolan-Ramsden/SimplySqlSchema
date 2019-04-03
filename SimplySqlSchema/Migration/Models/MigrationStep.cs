namespace SimplySqlSchema.Migration
{
    public class MigrationStep
    {
        public MigrationAction Action { get; set; }

        public MigrationTarget TargetType { get; set; }

        public string TargetName { get; set; }

        public override string ToString()
        {
            return $"{Action} {TargetName} (type {TargetType})";
        }
    }
}