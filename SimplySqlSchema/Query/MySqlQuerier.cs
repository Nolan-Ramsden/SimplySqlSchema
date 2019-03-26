namespace SimplySqlSchema.Query
{
    public class MySqlQuerier : SchemaQuerier
    {
        public override BackendType Backend => BackendType.MySql;
    }
}
