using System;
using System.Data;

namespace SimplySqlSchema.Attributes
{
    public class AliasTypeAttribute : Attribute
    {
        public SqlDbType AsType { get; }

        public AliasTypeAttribute(SqlDbType asType)
        {
            this.AsType = asType;
        }
    }
}
