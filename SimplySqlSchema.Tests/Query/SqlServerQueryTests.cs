using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.SqlServer;

namespace SimplySqlSchema.Tests.Query
{
    [TestClass]
    public class SqlServerQueryTests : SchemaQueryTestBase
    {
        [TestInitialize]
        public void Setup()
        {
            this.Query = new SqlServerQuerier();
            this.Manager = new SqlServerSchemaManager();
            this.Connection = new SqlConnection($@"Server=localhost\SQLEXPRESS;Database=Test;Integrated Security=true;");
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
