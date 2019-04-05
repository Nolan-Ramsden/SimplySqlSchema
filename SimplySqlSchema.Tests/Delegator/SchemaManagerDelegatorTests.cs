using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Delegator;
using SimplySqlSchema.Manager.Implementations;
using SimplySqlSchema.Tests.Common;

namespace SimplySqlSchema.Tests.Delegator
{
    [TestClass]
    public class SchemaManagerDelegatorTests
    {
        protected string TestDbFile { get; } = "DelegatorTest.db";
        protected SQLiteConnection Connection { get; set; }
        protected ISchemaManager DefaultManager { get; set; }
        protected ISchemaManager SqlLiteManager { get; set; }
        protected SchemaManagerDelegator Delegator { get; set; }

        [TestInitialize]
        public void Setup()
        {
            this.DefaultManager = new SqlSchemaManager();
            this.SqlLiteManager = new SQLiteSchemaManager();
            this.Connection = new SQLiteConnection($"Data Source={TestDbFile};Version=3;");
            this.Delegator = new SchemaManagerDelegator(
                managers: new [] { this.DefaultManager, this.SqlLiteManager }
            );
        }

        [TestMethod]
        public async Task TestGetManager()
        {
            var schema = new ObjectSchema()
            {
                Name = "TestTable",
                Columns = new Dictionary<string, ColumnSchema>()
                {
                    ["Key"] = new ColumnSchema()
                    {
                        Name = "Key",
                        SqlType = SqlDbType.Int,
                        KeyIndex = 1
                    }
                }
            };
            await this.SqlLiteManager.CreateObject(this.Connection, schema);
            var fetchedSchema = await this.Delegator.GetSchemaManager(BackendType.SQLite).GetSchema(this.Connection, schema.Name);

            SchemaAssertions.AssertObjectSchemasEqual(schema, fetchedSchema);
        }

        [TestMethod]
        public void TestGetNonExistantManager()
        {
            Assert.ThrowsException<NotImplementedException>(() => this.Delegator.GetSchemaManager(BackendType.SqlServer));
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Connection.Dispose();
            if (File.Exists(this.TestDbFile))
            {
                File.Delete(this.TestDbFile);
            }
        }
    }
}
