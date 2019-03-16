using System;

namespace SimplySqlSchema.Attributes
{
    public class AliasTypeAttribute : Attribute
    {
        public Type AsType { get; }

        public AliasTypeAttribute(Type asType)
        {
            this.AsType = asType;
        }
    }
}
