using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace SimplySqlSchema.Manager
{
    public class TypeMapper
    {
        public Dictionary<Type, List<SqlTypeAndDefault>> Mappings { get; } = new Dictionary<Type, List<SqlTypeAndDefault>>()
        {
            [typeof(int)] = new List<SqlTypeAndDefault>()
            {
                new SqlTypeAndDefault(SqlDbType.Int, "0"),
            },
            [typeof(bool)] = new List<SqlTypeAndDefault>()
            {
                new SqlTypeAndDefault(SqlDbType.Bit, "0"),
            },
            [typeof(double)] = new List<SqlTypeAndDefault>()
            {
                new SqlTypeAndDefault(SqlDbType.BigInt, "0"),
            },
            [typeof(DateTime)] = new List<SqlTypeAndDefault>()
            {
                new SqlTypeAndDefault(SqlDbType.DateTime, "0"),
                new SqlTypeAndDefault(SqlDbType.Timestamp, "0"),
                new SqlTypeAndDefault(SqlDbType.Date, "0"),
            },
            [typeof(string)] = new List<SqlTypeAndDefault>()
            {
                new SqlTypeAndDefault(SqlDbType.VarChar, "''", true),
            },
        };

        public SqlDbType GetSqlType(Type dotnetType)
        {
            if (!Mappings.ContainsKey(dotnetType))
            {
                throw new NotImplementedException($"No Sql type mapping for type {dotnetType.Name}");
            }
            return Mappings[dotnetType].First().SqlType;
        }

        public string GetDefaultValue(SqlDbType? sqlType)
        {
            if (sqlType == null)
            {
                throw new ArgumentNullException(nameof(sqlType));
            }

            var mapping = Mappings.Values.SelectMany(m => m).SingleOrDefault(s => s.SqlType == sqlType);
            if (mapping == null)
            {
                throw new NotImplementedException($"No dotnet type mapping for type {sqlType}");
            }
            return mapping.DefaultValue;
        }

        public bool IsLengthLimited(SqlDbType? sqlType)
        {
            if (sqlType == null)
            {
                throw new ArgumentNullException(nameof(sqlType));
            }

            var mapping = Mappings.Values.SelectMany(m => m).SingleOrDefault(s => s.SqlType == sqlType);
            if (mapping == null)
            {
                throw new NotImplementedException($"No dotnet type mapping for type {sqlType}");
            }
            return mapping.HasLength;
        }

        public TypeMapper Override(SqlDbType sqlDbType, string defaultValue, bool hasLength = false)
        {
            var mapping = new SqlTypeAndDefault(sqlDbType, defaultValue, hasLength);
            foreach(var keyVal in this.Mappings)
            {
                foreach(var typeAndDef in keyVal.Value)
                {
                    if (typeAndDef.SqlType == sqlDbType)
                    {
                        typeAndDef.DefaultValue = defaultValue;
                        typeAndDef.HasLength = hasLength;
                    }
                }
            }
            return this;
        }

        public class SqlTypeAndDefault
        {
            public SqlDbType SqlType { get; set; }

            public string DefaultValue { get; set; }

            public bool HasLength { get; set; }

            public SqlTypeAndDefault(SqlDbType sqlType, string defaultValue, bool hasLength = false)
            {
                this.SqlType = sqlType;
                this.DefaultValue = defaultValue;
                this.HasLength = hasLength;
            }
        }
    }
}
