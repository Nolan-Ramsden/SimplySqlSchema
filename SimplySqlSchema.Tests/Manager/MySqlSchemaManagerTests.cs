using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Manager.Implementations;
using MySql.Data.MySqlClient;

namespace SimplySqlSchema.Tests.Manager
{
    [TestClass]
    public class MySqlSchemaManagerTests : SqlManagerTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            this.Manager = new MySqlSchemaManager();
            this.Connection = new MySqlConnection("Server=127.0.0.1;Database=test;Uid=root;Pwd=password;");
        }

        [TestMethod]
        public async Task TestGetSchemaNonExistantSchema()
        {
            await this.BaseTestGetSchemaNonExistantSchema();
        }

        [TestMethod]
        public async Task TestGetSchemaCreatedSchema()
        {
            await this.BaseTestGetSchemaCreatedSchema();
        }

        [TestMethod]
        public async Task TestCreateColumnForAllTypes()
        {
            await this.BaseTestCreateColumnForAllTypes();
        }

        [TestMethod]
        public async Task TestAddColumnOnSchema()
        {
            await this.BaseTestAddColumnOnSchema();
        }

        [TestMethod]
        public async Task TestDeleteColumnOnSchema()
        {
            await this.BaseTestDeleteColumnOnSchema();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Manager.DeleteObject(this.Connection, this.BaselineSchema.Name).GetAwaiter().GetResult();
            this.Connection.Dispose();
        }
    }
}
