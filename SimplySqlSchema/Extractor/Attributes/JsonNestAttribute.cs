using System;
using System.Data;
using Newtonsoft.Json;

namespace SimplySqlSchema.Attributes
{
    public class JsonNestAttribute : Attribute
    {
        public int MaxLength { get; set; }

        public JsonNestAttribute(int maxLength = 1024)
        {
            this.MaxLength = maxLength;
        }
    }

    public class JsonNestMapper : Dapper.SqlMapper.ITypeHandler
    {
        public object Parse(Type destinationType, object value)
        {
            return JsonConvert.DeserializeObject(value.ToString(), destinationType);
        }

        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
