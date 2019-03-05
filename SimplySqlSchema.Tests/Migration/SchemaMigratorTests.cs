using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Manager.Implementations;
using SimplySqlSchema.Migration;
using SimplySqlSchema.Tests.Common;

namespace SimplySqlSchema.Tests.Migration
{
    [TestClass]
    public class SchemaMigratorTests
    {
        protected string TestDbFile { get; } = "MigratorTest.db";
        protected SchemaMigrator Migrator { get; set; }
        protected MigrationOptions Options { get; set; }
        protected SQLiteSchemaManager Manager { get; set; }
        protected SQLiteConnection Connection { get; set; }
        protected ObjectSchema BaselineSchema { get; set; }

        [TestInitialize]
        public void Setup()
        {
            this.Migrator = new SchemaMigrator();
            this.Manager = new SQLiteSchemaManager();
            this.Connection = new SQLiteConnection($"Data Source={TestDbFile};Version=3;");
            this.BaselineSchema = new ObjectSchema()
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
                        Type = typeof(string)
                    },
                }.ToDictionary(c => c.Name)
            };
            this.Options = new MigrationOptions()
            {
                AllowDataloss = true
            };
        }

        [TestMethod]
        public async Task TestCreateSchema()
        {
            var plan = await this.Migrator.PlanMigration(
                connection: this.Connection,
                targetManager: this.Manager,
                targetSchema: this.BaselineSchema,
                options: this.Options
            );

            Assert.IsNull(plan.Existing, "Existing schema should be null");
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, plan.Target);
            Assert.AreEqual(1, plan.Steps.Count, "Only plan step should be create");
            Assert.AreEqual(MigrationAction.Create, plan.Steps[0].Action, "Only action should be create");
            Assert.AreEqual(this.BaselineSchema.Name, plan.Steps[0].TargetName, "Only step should target the table");
            Assert.AreEqual(MigrationTarget.Object, plan.Steps[0].TargetType, "Only step should target the table");

            await this.Migrator.Execute(
                targetManager: this.Manager,
                connection: this.Connection,
                plan: plan
            );

            var migratedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, migratedSchema);
        }

        [TestMethod]
        public async Task TestAddColumnToSchema()
        {
            await this.TestCreateSchema();

            this.BaselineSchema.Columns["NewColumn"] = new ColumnSchema()
            {
                Name = "NewColumn",
                Type = typeof(DateTime)
            };

            var plan = await this.Migrator.PlanMigration(
                connection: this.Connection,
                targetManager: this.Manager,
                targetSchema: this.BaselineSchema,
                options: this.Options
            );

            Assert.IsNotNull(plan.Existing, "Existing schema should not be null");
            Assert.AreEqual(1, plan.Steps.Count, "Only plan step should be create column");
            Assert.AreEqual(MigrationAction.Create, plan.Steps[0].Action, "Only action should be create column");
            Assert.AreEqual("NewColumn", plan.Steps[0].TargetName, "Only step should target the column");
            Assert.AreEqual(MigrationTarget.Column, plan.Steps[0].TargetType, "Only step should target the column");

            await this.Migrator.Execute(
                targetManager: this.Manager,
                connection: this.Connection,
                plan: plan
            );

            var migratedSchema = await this.Manager.GetSchema(this.Connection, this.BaselineSchema.Name);
            SchemaAssertions.AssertObjectSchemasEqual(this.BaselineSchema, migratedSchema);
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
