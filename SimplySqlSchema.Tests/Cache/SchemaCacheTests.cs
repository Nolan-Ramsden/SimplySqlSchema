using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Cache;
using SimplySqlSchema.Manager.Implementations;
using SimplySqlSchema.Tests.Common;

namespace SimplySqlSchema.Tests.Cache
{
    [TestClass]
    public class SchemaCacheTests
    {
        protected string TestDbFile { get; } = "CacheTest.db";
        protected ISchemaCache SchemaCache { get; set; }
        protected SQLiteConnection Connection { get; set; }
        protected ObjectSchema BaselineSchema { get; set; }
        protected CachedSchemaManager CachedManager { get; set; }
        protected SQLiteSchemaManager SqlLiteManager { get; set; }

        [TestInitialize]
        public void Setup()
        {
            this.SchemaCache = new DictionarySchemaCache();
            this.SqlLiteManager = new SQLiteSchemaManager();
            this.Connection = new SQLiteConnection($"Data Source={TestDbFile};Version=3;");
            this.CachedManager = new CachedSchemaManager(
                impl: this.SqlLiteManager,
                schemaCache: this.SchemaCache
            );
            this.BaselineSchema = new ObjectSchema()
            {
                Name = "TestObject",
                Columns = new List<ColumnSchema>()
                {
                    new ColumnSchema()
                    {
                        Name = "Id",
                        KeyIndex = 1,
                        Nullable = false,
                        SqlType = SqlDbType.Int
                    },
                    new ColumnSchema()
                    {
                        Name = "Val",
                        SqlType = SqlDbType.NVarChar,
                        MaxLength = 100
                    },
                }.ToDictionary(c => c.Name)
            };
        }

        [TestMethod]
        public async Task TestGetSchemaIsCached()
        {
            await this.CachedManager.CreateObject(this.Connection, this.BaselineSchema);
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, this.SchemaCache.Get(this.BaselineSchema.Name));

            await this.SqlLiteManager.DeleteObject(this.Connection, this.BaselineSchema.Name);

            var fetchedSchema = await this.CachedManager.GetSchema(this.Connection, this.BaselineSchema.Name);
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, fetchedSchema);
        }

        [TestMethod]
        public async Task TestUpdateSchemaIsUpdatedIntoCache()
        {
            await this.CachedManager.CreateObject(this.Connection, this.BaselineSchema);
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, this.SchemaCache.Get(this.BaselineSchema.Name));

            var newCol = new ColumnSchema()
            {
                Name = "NewCol",
                SqlType = SqlDbType.Bit
            };
            this.BaselineSchema.Columns["NewCol"] = newCol;
            await this.CachedManager.CreateColumn(this.Connection, this.BaselineSchema.Name, newCol);

            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, this.SchemaCache.Get(this.BaselineSchema.Name));
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
