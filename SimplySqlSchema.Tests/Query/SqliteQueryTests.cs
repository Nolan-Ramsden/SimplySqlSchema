using System.IO;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.SQLite;

namespace SimplySqlSchema.Tests.Query
{
    [TestClass]
    public class SQLiteQueryTests : SchemaQueryTestBase
    {
        protected string TestDbFile { get; } = "QueryTest.db";

        [TestInitialize]
        public void Setup()
        {
            this.Query = new SQLiteQuerier();
            this.Manager = new SQLiteSchemaManager();
            this.Connection = new SQLiteConnection($"Data Source={TestDbFile};Version=3;");
        }

        [TestMethod]
        public async Task TestInsertMultipleGetEntity()
        {
            await base.BaseTestInsertMultipleGetEntity();
        }

        [TestMethod]
        public async Task TestUpdateEntity()
        {
            await base.BaseTestUpdateEntity();
        }

        [TestMethod]
        public async Task TestDeleteEntity()
        {
            await this.BaseTestDeleteEntity();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Manager.DeleteObject(this.Connection, this.BaselineSchema.Name).GetAwaiter().GetResult();
            this.Connection.Dispose();
            if (File.Exists(this.TestDbFile))
            {
                File.Delete(this.TestDbFile);
            }
        }
    }
}
