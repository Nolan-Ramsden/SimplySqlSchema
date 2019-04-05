using System.IO;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Manager.Implementations;

namespace SimplySqlSchema.Tests.Manager
{
    [TestClass]
    public class SQLiteSchemaManagerTests : SqlManagerTestBase
    {
        protected string TestDbFile { get; } = "ManagerTest.db";

        [TestInitialize]
        public void Setup()
        {
            this.Manager = new SQLiteSchemaManager();
            this.Connection = new SQLiteConnection($"Data Source={TestDbFile};Version=3;");
        }

        [TestMethod]
        public async Task TestGetSchemaNonExistantSchema()
        {
            await base.BaseTestGetSchemaNonExistantSchema();
        }

        [TestMethod]
        public async Task TestGetSchemaCreatedSchema()
        {
            await base.BaseTestGetSchemaCreatedSchema();
        }

        [TestMethod]
        public async Task TestCreateColumnForAllTypes()
        {
            await this.BaseTestCreateColumnForAllTypes();
        }

        [TestMethod]
        public async Task TestAddColumnOnSchema()
        {
            await base.BaseTestAddColumnOnSchema();
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
