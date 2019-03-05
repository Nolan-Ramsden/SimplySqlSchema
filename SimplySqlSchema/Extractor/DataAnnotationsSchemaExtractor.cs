using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SimplySqlSchema.Extractor
{
    public class DataAnnotationsSchemaExtractor : IObjectSchemaExtractor
    {
        public ObjectSchema GetObjectSchema(Type t, string objectName)
        {
            if (objectName == null)
            {
                var tableAttribute = t.GetCustomAttribute<TableAttribute>();
                if (tableAttribute != null)
                {
                    objectName = tableAttribute.Name;
                }
                if (objectName == null)
                {
                    throw new InvalidOperationException($"Unable to infer tablename of {t.Name}");
                }
            }

            var index = 1;
            var columns = new List<ColumnSchema>();
            foreach(var property in t.GetProperties())
            {
                var column = ParseProperty(property, index);
                if (column.KeyIndex > 0)
                {
                    index++;
                }
                columns.Add(column);
            }

            return new ObjectSchema()
            {
                Name = objectName,
                Indexes = new Dictionary<string, IndexSchema>(),
                Columns = columns.ToDictionary(c => c.Name),
            };
        }

        protected ColumnSchema ParseProperty(PropertyInfo p, int keyIndex)
        {
            bool isKey = p.GetCustomAttribute<KeyAttribute>() != null;
            return new ColumnSchema()
            {
                Name = p.Name,
                Type = p.PropertyType,
                Nullable = !p.PropertyType.IsValueType && !isKey,
                MaxLength = p.GetCustomAttribute<MaxLengthAttribute>()?.Length,
                KeyIndex = isKey ? (int?)keyIndex : null,
            };
        }
    }
}
