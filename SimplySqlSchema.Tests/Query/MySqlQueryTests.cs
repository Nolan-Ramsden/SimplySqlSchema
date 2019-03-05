using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using SimplySqlSchema.Manager.Implementations;
using SimplySqlSchema.Query;

namespace SimplySqlSchema.Tests.Query
{
    [TestClass]
    public class MySqlQueryTests : SchemaQueryTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            this.Query = new MySqlQuerier();
            this.Manager = new MySqlSchemaManager();
            this.Connection = new MySqlConnection("Server=127.0.0.1;Database=test;Uid=root;Pwd=password;");
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
        }
    }
}
