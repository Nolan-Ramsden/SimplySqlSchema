using System;

namespace SimplySqlSchema
{
    public class ColumnSchema
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public bool Nullable { get; set; }

        public int? KeyIndex { get; set; }

        public int? MaxLength { get; set; }
    }
}