using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Tests.Common;

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
                        Type = typeof(int)
                    },
                    new ColumnSchema()
                    {
                        Name = "Val",
                        Type = typeof(string),
                        MaxLength = 100
                    },
                    new ColumnSchema()
                    {
                        Name = "UpdatedAt",
                        Type = typeof(DateTime),
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
                Type = typeof(DateTime)
            };
            this.BaselineSchema.Columns[newColumn.Name] = newColumn;
            await this.Manager.CreateColumn(this.Connection, this.BaselineSchema.Name, newColumn);

            var fetchedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);

            Assert.IsNotNull(fetchedSchema, "Existant schema should return non-null");
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, fetchedSchema);
        }

        public async Task BaseTestDeleteColumnOnSchema()
        {
            await this.Manager.CreateObject(this.Connection, this.BaselineSchema);
            var newColumn = new ColumnSchema()
            {
                Name = "NewColumn",
                Type = typeof(DateTime)
            };
            this.BaselineSchema.Columns.Remove("Val");
            await this.Manager.DeleteColumn(this.Connection, this.BaselineSchema.Name, "Val");

            var fetchedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);

            Assert.IsNotNull(fetchedSchema, "Existant schema should return non-null");
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, fetchedSchema);
        }
    }
}
