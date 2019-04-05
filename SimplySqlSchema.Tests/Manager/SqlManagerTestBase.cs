using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Tests.Common;
using SimplySqlSchema.Manager;

namespace SimplySqlSchema.Tests.Manager
{
    public class SqlManagerTestBase
    {
        protected ISchemaManager Manager { get; set; }
        protected IDbConnection Connection { get; set; }
        protected ObjectSchema BaselineSchema { get; set; } = new ObjectSchema()
        {
            Name = "TestObject",
            Columns = new List<ColumnSchema>()
                {
                    new ColumnSchema()
                    {
                        Name = "Id",
                        KeyIndex = 1,
                        SqlType = SqlDbType.Int
                    },
                    new ColumnSchema()
                    {
                        Name = "Val",
                        SqlType = SqlDbType.VarChar,
                        MaxLength = 100
                    },
                    new ColumnSchema()
                    {
                        Name = "IsReal",
                        SqlType = SqlDbType.Bit,
                    },
                    new ColumnSchema()
                    {
                        Name = "UpdatedAt",
                        SqlType = SqlDbType.DateTime,
                        Nullable = true
                    },
                }.ToDictionary(c => c.Name)
        };

        public async Task BaseTestGetSchemaNonExistantSchema()
        {
            var fetchedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);

            Assert.IsNull(fetchedSchema, "Non-existant schema should return null");
        }

        public async Task BaseTestGetSchemaCreatedSchema()
        {
            await this.Manager.CreateObject(this.Connection, this.BaselineSchema);
            var fetchedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);

            Assert.IsNotNull(fetchedSchema, "Existant schema should return non-null");
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, fetchedSchema);
        }

        public async Task BaseTestAddColumnOnSchema()
        {
            await this.Manager.CreateObject(this.Connection, this.BaselineSchema);
            var newColumn = new ColumnSchema()
            {
                Name = "NewColumn",
                SqlType = SqlDbType.Date
            };
            this.BaselineSchema.Columns[newColumn.Name] = newColumn;
            await this.Manager.CreateColumn(this.Connection, this.BaselineSchema.Name, newColumn);

            var fetchedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);

            Assert.IsNotNull(fetchedSchema, "Existant schema should return non-null");
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, fetchedSchema);
        }

        public async Task BaseTestCreateColumnForAllTypes()
        {
            var mapper = new TypeMapper();
            var jumboSchema = new ObjectSchema()
            {
                Name = this.BaselineSchema.Name
            };
            jumboSchema.Columns["Id"] = new ColumnSchema()
            {
                Name = "Id",
                KeyIndex = 1,
                SqlType = SqlDbType.Int
            };
            foreach (var type in mapper.Mappings.SelectMany(m => m.Value))
            {
                string name = $"Column{type.SqlType.ToString()}";
                jumboSchema.Columns[name] = new ColumnSchema()
                {
                    Name = name,
                    Nullable = false,
                    SqlType = type.SqlType,
                    MaxLength = type.HasLength ? (int?)100 : null
                };
            }

            await this.Manager.CreateObject(this.Connection, jumboSchema);
            var fetchedSchema = await this.Manager.GetSchema(this.Connection, jumboSchema.Name);

            Assert.IsNotNull(fetchedSchema, "Existant schema should return non-null");
            SchemaAssertions.AssertObjectSchemasEqual(jumboSchema, fetchedSchema);
        }

        public async Task BaseTestDeleteColumnOnSchema()
        {
            await this.Manager.CreateObject(this.Connection, this.BaselineSchema);
            var newColumn = new ColumnSchema()
            {
                Name = "NewColumn",
                SqlType = SqlDbType.Date
            };
            this.BaselineSchema.Columns.Remove("Val");
            await this.Manager.DeleteColumn(this.Connection, this.BaselineSchema.Name, "Val");

            var fetchedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);

            Assert.IsNotNull(fetchedSchema, "Existant schema should return non-null");
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, fetchedSchema);
        }
    }
}
