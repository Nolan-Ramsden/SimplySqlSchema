namespace SimplySqlSchema
{
    public class MigrationOptions
    {
        public bool AllowDataloss { get; set; } = false;

        public bool ForceDropRecreate { get; set; } = false;

        public bool Freeze { get; set; } = false;
    }
}
