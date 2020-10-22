using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Attributes;
using SimplySqlSchema.Cache;
using SimplySqlSchema.Delegator;
using SimplySqlSchema.Extractor;
using SimplySqlSchema.Migration;
using SimplySqlSchema.SQLite;

namespace SimplySqlSchema.Tests
{
    [TestClass]
    public class EndToEndTests
    {
        protected string SchemaName { get; } = "TestSchema";
        protected string TestDbFile { get; } = "QueryTest.db";

        protected BackendType Backend { get; set; }
        protected IDbConnection Connection { get; set; }
        protected ISimplySqlSchema Delegator { get; set; }

        [TestInitialize]
        public void Setup()
        {
            this.Backend = BackendType.SQLite;
            this.Delegator = new ProviderDelegator(
                migrator: new SchemaMigrator(),
                managers: new[] { new SQLiteSchemaManager() },
                queriers: new[] { new SQLiteQuerier() },
                extractor: new DataAnnotationsSchemaExtractor(),
                schemaCache: new DictionarySchemaCache(),
                connectors: new[] { new SQLiteConnectionFactory() }
            );
            this.Connection = this.Delegator
                .GetConnectionFactory(this.Backend)
                .GetConnection($"Data Source={TestDbFile};Version=3;");
        }

        [TestMethod]
        public async Task TestBasicFlow()
        {
            var schema = this.Delegator.GetExtractor().GetObjectSchema(typeof(TestUser), this.SchemaName);

            await this.Delegator.GetMigrator().PlanAndMigrate(
                connection: this.Connection,
                targetManager: this.Delegator.GetManager(this.Backend),
                targetSchema: schema,
                options: null
            );

            var nolan = new TestUser()
            {
                Name = "Nolan",
                Age = 24,
                LastUpdated = DateTime.Now,
                Address = new TestAddress()
                {
                    ApartmentNumber = 211,
                    IsApartment = true,
                    Street = "Not tellin ya"
                },
                Attributes = new List<TestAttribute>()
                {
                    new TestAttribute()
                    {
                        Attribute = "Hair-Colour",
                        Value = "Blonde"
                    },
                    new TestAttribute()
                    {
                        Attribute = "Eye-Colour",
                        Value = "Green"
                    },
                }
            };

            await this.Delegator.GetQuerier(this.Backend).Insert(
                connection: this.Connection,
                schema: schema,
                obj: nolan
            );

            var fetchedNolan = await this.Delegator.GetQuerier(this.Backend).Get(
                connection: this.Connection,
                schema: schema,
                keyedObject: new TestUser()
                {
                    Age = nolan.Age,
                    Name = nolan.Name
                }
            );

            Assert.IsNotNull(fetchedNolan);
            Assert.AreEqual(nolan.Name, fetchedNolan.Name);
            Assert.AreEqual(nolan.Age, fetchedNolan.Age);
            Assert.AreEqual(nolan.LastUpdated, fetchedNolan.LastUpdated);
            Assert.AreEqual(nolan.Address.Street, fetchedNolan.Address.Street);
            Assert.AreEqual(nolan.Address.IsApartment, fetchedNolan.Address.IsApartment);
            Assert.AreEqual(nolan.Address.ApartmentNumber, fetchedNolan.Address.ApartmentNumber);
            Assert.AreEqual(nolan.Attributes.Count, fetchedNolan.Attributes.Count);
            Assert.AreEqual(nolan.Attributes[0].Attribute, fetchedNolan.Attributes[0].Attribute);
            Assert.AreEqual(nolan.Attributes[0].Value, fetchedNolan.Attributes[0].Value);
            Assert.AreEqual(nolan.Attributes[1].Attribute, fetchedNolan.Attributes[1].Attribute);
            Assert.AreEqual(nolan.Attributes[1].Value, fetchedNolan.Attributes[1].Value);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Delegator.GetManager(this.Backend)
                .DeleteObject(this.Connection, this.SchemaName)
                .GetAwaiter().GetResult();
            this.Connection.Dispose();
            if (File.Exists(this.TestDbFile))
            {
                File.Delete(this.TestDbFile);
            }
        }

        class TestUser
        {
            [Key]
            public int Age { get; set; }

            [Key]
            public string Name { get; set; }

            public DateTime LastUpdated { get; set; }

            [JsonNest]
            public TestAddress Address { get; set; }

            [JsonNest]
            public List<TestAttribute> Attributes { get; set; }
        }

        class TestAddress
        {
            public string Street { get; set; }

            public bool IsApartment { get; set; }

            public int? ApartmentNumber { get; set; }
        }

        class TestAttribute
        {
            public string Attribute { get; set; }

            public string Value { get; set; }
        }
    }
}
