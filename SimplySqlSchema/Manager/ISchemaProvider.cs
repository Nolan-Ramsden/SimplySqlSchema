using System.Data;
using System.Threading.Tasks;

namespace SimplySqlSchema
{
    public interface ISchemaProvider
    {
        BackendType Backend { get; }

        Task<ObjectSchema> GetSchema(IDbConnection connection, string objectName);
    }
}
