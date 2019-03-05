using System.Collections.Generic;

namespace SimplySqlSchema
{
    public class ObjectSchema
    {
        public string Name { get; set; }

        public IDictionary<string, ColumnSchema> Columns { get; set; } = new Dictionary<string, ColumnSchema>();

        public IDictionary<string, IndexSchema> Indexes { get; set; } = new Dictionary<string, IndexSchema>();
    }
}
