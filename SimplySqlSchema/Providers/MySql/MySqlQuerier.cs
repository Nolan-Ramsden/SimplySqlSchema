using SimplySqlSchema.Query;

namespace SimplySqlSchema.MySql
{
    public class MySqlQuerier : SchemaQuerier
    {
        public override BackendType Backend => BackendType.MySql;
    }
}
