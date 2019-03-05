using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimplySqlSchema.Tests.Common
{
    public class SchemaAssertions
    {
        public static void AssertObjectSchemasEqual(ObjectSchema expected, ObjectSchema actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, "Object names don't match");
            Assert.AreEqual(expected.Indexes.Count, actual.Indexes.Count, "Index counts don't match");
            Assert.AreEqual(expected.Columns.Count, actual.Columns.Count, "Column counts don't match");

            foreach(var column in expected.Columns.Keys.Union(actual.Columns.Keys))
            {
                Assert.IsTrue(actual.Columns.ContainsKey(column), $"Column '{column}' in expected but not actual");
                Assert.IsTrue(expected.Columns.ContainsKey(column), $"Column '{column}' in actual but not expected");

                AssertColumnSchemasEqual(expected.Columns[column], actual.Columns[column]);
            }

            foreach (var index in expected.Indexes.Keys.Union(actual.Indexes.Keys))
            {
                Assert.IsTrue(actual.Indexes.ContainsKey(index), $"Index '{index}' in expected but not actual");
                Assert.IsTrue(expected.Indexes.ContainsKey(index), $"Index '{index}' in actual but not expected");

                AssertIndexSchemasEqual(expected.Indexes[index], actual.Indexes[index]);
            }
        }

        public static void AssertColumnSchemasEqual(ColumnSchema a, ColumnSchema b)
        {
            Assert.AreEqual(a.Name, b.Name, $"Column '{a.Name}' names don't match");
            Assert.AreEqual(a.Type, b.Type, $"Column '{a.Name}' types don't match");
            Assert.AreEqual(a.KeyIndex, b.KeyIndex, $"Column '{a.Name}' key indexes don't match");
            Assert.AreEqual(a.Nullable, b.Nullable, $"Column '{a.Name}' nullability don't match");
            Assert.AreEqual(a.MaxLength, b.MaxLength, $"Column '{a.Name}' max lengths don't match");
        }

        public static void AssertIndexSchemasEqual(IndexSchema a, IndexSchema b)
        {
            Assert.AreEqual(a.Name, b.Name, "Index names don't match");
        }
    }
}
