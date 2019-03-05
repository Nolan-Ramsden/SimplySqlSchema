using System.Data;
using System.Threading.Tasks;

namespace SimplySqlSchema
{
    public interface ISchemaQuerier
    {
        BackendType Backend { get; }

        Task<T> Get<T>(IDbConnection connection, ObjectSchema schema, T keyedObject);

        Task Insert<T>(IDbConnection connection, ObjectSchema schema, T obj);

        Task Update<T>(IDbConnection connection, ObjectSchema schema, T obj);

        Task Delete<T>(IDbConnection connection, ObjectSchema schema, T keyedObject);
    }
}
