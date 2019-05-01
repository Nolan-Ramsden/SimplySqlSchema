using System;
using System.Linq;
using System.Data;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using SimplySqlSchema.Attributes;

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
            foreach(var property in t.GetProperties().Where(p => p.GetCustomAttribute<IgnorePropertyAttribute>() == null))
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
            Type dotnetType = p.PropertyType;
            Type underlyingType = Nullable.GetUnderlyingType(dotnetType);
            if (underlyingType != null)
            {
                dotnetType = underlyingType;
            }
            bool isKey = p.GetCustomAttribute<KeyAttribute>() != null;
            int? pk = isKey ? (int?)keyIndex : null;

            var jsonNested = p.GetCustomAttribute<JsonNestAttribute>();
            if (jsonNested != null)
            {
                return this.CreateJsonNestedColumn(p, pk, jsonNested, dotnetType);
            }

            int? maxLength = p.GetCustomAttribute<MaxLengthAttribute>()?.Length;
            SqlDbType? sqlType = p.GetCustomAttribute<AliasTypeAttribute>()?.AsType;
            if (sqlType == null)
            {
                if (dotnetType.IsEnum)
                {
                    sqlType = SqlDbType.VarChar;
                    maxLength = 128;
                }
            }

            bool nullable = underlyingType != null || dotnetType == typeof(string);
            if (pk.HasValue)
            {
                nullable = false;
            }

            return new ColumnSchema()
            {
                Name = p.Name,
                DotnetType = dotnetType,
                Nullable = nullable,
                MaxLength = maxLength,
                KeyIndex = pk,
                SqlType = sqlType
            };
        }

        protected ColumnSchema CreateJsonNestedColumn(PropertyInfo p, int? keyIndex, JsonNestAttribute jsonNest, Type dotnetType)
        {
            Dapper.SqlMapper.AddTypeHandler(dotnetType, new JsonNestMapper());
            return new ColumnSchema()
            {
                Name = p.Name,
                DotnetType = dotnetType,
                Nullable = !keyIndex.HasValue,
                MaxLength = jsonNest.MaxLength,
                KeyIndex = keyIndex,
                SqlType = SqlDbType.VarChar
            };
        }
    }
}
