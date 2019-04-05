using System;
using System.Data;

namespace SimplySqlSchema
{
    public class ColumnSchema
    {
        public string Name { get; set; }

        public Type DotnetType { get; set; }

        public SqlDbType? SqlType { get; set; }

        public bool Nullable { get; set; }

        public int? KeyIndex { get; set; }

        public int? MaxLength { get; set; }
    }
}