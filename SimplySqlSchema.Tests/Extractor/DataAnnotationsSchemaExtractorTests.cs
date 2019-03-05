using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplySqlSchema.Extractor;
using SimplySqlSchema.Tests.Common;

namespace SimplySqlSchema.Tests.Extractor
{
    [TestClass]
    public class DataAnnotationsSchemaExtractorTests
    {
        protected ObjectSchema ExpectedSchema { get; set; }
        protected DataAnnotationsSchemaExtractor Extractor { get; set; }

        [TestInitialize]
        public void Setup()
        {
            this.Extractor = new DataAnnotationsSchemaExtractor();
            this.ExpectedSchema = new ObjectSchema()
            {
                Name = "TestTable",
                Columns = new List<ColumnSchema>()
                {
                    new ColumnSchema()
                    {
                        Name = "Key1",
                        KeyIndex = 1,
                        Type = typeof(int),
                    },
                    new ColumnSchema()
                    {
                        Name = "Key2",
                        KeyIndex = 2,
                        Type = typeof(string),
                    },
                    new ColumnSchema()
                    {
                        Name = "CreatedAt",
                        Type = typeof(DateTime),
                    },
                    new ColumnSchema()
                    {
                        Name = "Val1",
                        Nullable = true,
                        Type = typeof(string),
                    },
                    new ColumnSchema()
                    {
                        Name = "Val2",
                        Nullable = true,
                        MaxLength = 100,
                        Type = typeof(string),
                    }
                }.ToDictionary(c => c.Name)
            };
        }

        [TestMethod]
        public void TestParseObjectWithName()
        {
            var extractedSchema = this.Extractor.GetObjectSchema(typeof(TestObject), "TestTable");

            SchemaAssertions.AssertObjectSchemasEqual(this.ExpectedSchema, extractedSchema);
        }

        [TestMethod]
        public void TestParseObjectWithoutName()
        {
            var extractedSchema = this.Extractor.GetObjectSchema(typeof(TestObject), null);

            SchemaAssertions.AssertObjectSchemasEqual(this.ExpectedSchema, extractedSchema);
        }

        [Table("TestTable")]
        class TestObject
        {
            [Key]
            public int Key1 { get; set; }

            [Key]
            public string Key2 { get; set; }

            public DateTime CreatedAt { get; set; }

            public string Val1 { get; set; }

            [MaxLength(100)]
            public string Val2 { get; set; }
        }
    }
}
