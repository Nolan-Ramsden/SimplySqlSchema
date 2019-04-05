using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Manager.Implementations;

namespace SimplySqlSchema.Tests.Manager
{
    [TestClass]
    public class SqlServerSchemaManagerTests : SqlManagerTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            this.Manager = new SqlServerSchemaManager();
            this.Connection = new SqlConnection($@"Server=localhost\SQLEXPRESS;Database=Test;Integrated Security=true;");
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
