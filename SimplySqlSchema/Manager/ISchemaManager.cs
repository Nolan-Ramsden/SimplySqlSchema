using System;
using System.Data;
using System.Threading.Tasks;

namespace SimplySqlSchema
{
    public interface ISchemaManager : ISchemaProvider
    {
        Task CreateObject(IDbConnection connection, ObjectSchema objectSchema);

        Task DeleteObject(IDbConnection connection, string objectName);

        Task CreateColumn(IDbConnection connection, string objectName, ColumnSchema columnSchema);

        Task DeleteColumn(IDbConnection connection, string objectName, string columnName);

        Task CreateIndex(IDbConnection connection, string objectName, IndexSchema indexSchema);

        Task DeleteIndex(IDbConnection connection, string objectName, string indexName);

        SqlDbType MapColumnType(Type dotnetType);
    }
}
