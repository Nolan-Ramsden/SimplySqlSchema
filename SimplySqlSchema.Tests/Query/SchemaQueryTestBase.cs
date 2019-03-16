using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Query;

namespace SimplySqlSchema.Tests.Query
{
    public class SchemaQueryTestBase
    {
        protected SchemaQuerier Query { get; set; }
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
                        Name = "EnumVal",
                        Type = typeof(int),
                    },
                }.ToDictionary(c => c.Name)
        };
        protected QueryObject TestObject { get; set; } = new QueryObject()
        {
            Id = 15,
            Val = "15",
            EnumVal = TestEnum.B
        };

        public async Task BaseTestInsertMultipleGetEntity()
        {
            await this.Manager.CreateObject(this.Connection, this.BaselineSchema);
            await this.Query.Insert(this.Connection, this.BaselineSchema, this.TestObject);
            await this.Query.Insert(this.Connection, this.BaselineSchema, new QueryObject()
            {
                Id = 12,
                Val = 12.ToString(),
                EnumVal = TestEnum.A
            });

            var fetched = await this.Query.Get<QueryObject>(this.Connection, this.BaselineSchema, this.TestObject);

            Assert.IsNotNull(fetched, "Fetched object should not be null");
            Assert.AreEqual(15, fetched.Id, "Fetched object doesn't matched inserted");
            Assert.AreEqual("15", fetched.Val, "Fetched object doesn't matched inserted");
            Assert.AreEqual(TestEnum.B, fetched.EnumVal, "Fetched object doesn't matched inserted");
        }

        public async Task BaseTestUpdateEntity()
        {
            await this.BaseTestInsertMultipleGetEntity();
            this.TestObject.Val = "100";
            this.TestObject.EnumVal = TestEnum.C;

            await this.Query.Update(this.Connection, this.BaselineSchema, this.TestObject);
            var fetched = await this.Query.Get<QueryObject>(this.Connection, this.BaselineSchema, this.TestObject);

            Assert.IsNotNull(fetched, "Fetched object should not be null");
            Assert.AreEqual(15, fetched.Id, "Fetched object doesn't matched inserted");
            Assert.AreEqual("100", fetched.Val, "Fetched object doesn't matched inserted");
            Assert.AreEqual(TestEnum.C, fetched.EnumVal, "Fetched object doesn't matched inserted");
        }

        public async Task BaseTestDeleteEntity()
        {
            await this.BaseTestInsertMultipleGetEntity();

            await this.Query.Delete(this.Connection, this.BaselineSchema, this.TestObject);
            var fetched = await this.Query.Get<QueryObject>(this.Connection, this.BaselineSchema, this.TestObject);

            Assert.IsNull(fetched, "Fetched object should not exist");
        }

        public class QueryObject
        {
            public int Id { get; set; }

            public string Val { get; set; }

            public TestEnum EnumVal { get; set; }
        }

        public enum TestEnum
        {
            A = 1,
            B = 2,
            C = 3
        }
    }
}
