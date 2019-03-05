using System;
using System.Linq;
using System.Collections.Generic;

namespace SimplySqlSchema.Manager
{
    public class TypeMapper
    {
        static List<TypeMapping> Mappings { get; set; } = new List<TypeMapping>()
        {
            new TypeMapping(typeof(int))
                .Add("INT", "0"),
            new TypeMapping(typeof(bool))
                .Add("BOOLEAN", "0"),
            new TypeMapping(typeof(double))
                .Add("DOUBLE", "0"),
            new TypeMapping(typeof(DateTime))
                .Add("DATETIME", "0")
                .Add("TIMESTAMP", "0")
                .Add("DATE", "0"),
            new TypeMapping(typeof(string))
                .Add("VARCHAR", "''")
                .Add("NVARCHAR", "''"),
        };

        public TypeMapper Override(Type dotnetType, string sqlType, string defaultExpression)
        {
            sqlType = sqlType.ToUpper();
            var mapping = Mappings.SingleOrDefault(m => m.DotnetType == dotnetType);
            if (mapping == null)
            {
                mapping = new TypeMapping(dotnetType)
                    .Add(sqlType, defaultExpression);
                Mappings.Add(mapping);
                return this;
            }

            var existingSqlType = mapping.SqlTypes.SingleOrDefault(t => t.TypeName == sqlType);
            if (existingSqlType == null)
            {
                mapping.Add(sqlType, defaultExpression);
                return this;
            }

            existingSqlType.DefaultValueExpression = defaultExpression;
            return this;
        }

        public string GetSqlType(Type dotnetType)
        {
            var mapping = Mappings.SingleOrDefault(m => m.DotnetType == dotnetType);
            if (mapping == null)
            {
                throw new NotImplementedException($"No Sql type mapping for type {dotnetType.Name}");
            }
            return mapping.SqlTypes.First().TypeName;
        }

        public Type GetDotnetType(string sqlType)
        {
            sqlType = sqlType.ToUpper();
            var mapping = Mappings.SingleOrDefault(m => m.SqlTypes.Any(s => s.TypeName == sqlType));
            if (mapping == null)
            {
                throw new NotImplementedException($"No dotnet type mapping for type {sqlType}");
            }
            return mapping.DotnetType;
        }

        public string GetDefaultExpression(string sqlType)
        {
            sqlType = sqlType.ToUpper();
            var mapping = Mappings.SelectMany(m => m.SqlTypes).SingleOrDefault(s => s.TypeName == sqlType);
            if (mapping == null)
            {
                throw new NotImplementedException($"No dotnet type mapping for type {sqlType}");
            }
            return mapping.DefaultValueExpression;
        }

        class TypeMapping
        {
            public Type DotnetType { get; set; }

            public List<SqlType> SqlTypes { get; set; } = new List<SqlType>();

            public TypeMapping(Type dotnetType)
            {
                this.DotnetType = dotnetType;
            }

            public TypeMapping Add(string typeName, string expr)
            {
                this.SqlTypes.Add(new SqlType(typeName, expr));
                return this;
            }
        }

        class SqlType
        {
            public string TypeName { get; set; }

            public string DefaultValueExpression { get; set; }

            public SqlType(string typeName, string expr)
            {
                this.TypeName = typeName;
                this.DefaultValueExpression = expr;
            }
        }
    }
}
