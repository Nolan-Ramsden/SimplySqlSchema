using System;
using System.Linq;
using System.Collections.Generic;
using SimplySqlSchema.Cache;

namespace SimplySqlSchema.Delegator
{
    public class ProviderDelegator : ISimplySqlSchema
    {
        protected ISchemaCache SchemaCache { get; }
        protected ISchemaMigrator Migrator { get; }
        protected IObjectSchemaExtractor Extractor { get; }
        protected IEnumerable<ISchemaQuerier> Queriers { get; }
        protected IEnumerable<ISchemaManager> Managers { get; }
        protected IEnumerable<IConnectionFactory> Connectors { get; }
        
        public ProviderDelegator(
            ISchemaCache schemaCache,
            ISchemaMigrator migrator,
            IObjectSchemaExtractor extractor,
            IEnumerable< ISchemaManager> managers, 
            IEnumerable<ISchemaQuerier> queriers, 
            IEnumerable<IConnectionFactory> connectors
            )
        {
            this.Migrator = migrator;
            this.Managers = managers;
            this.Queriers = queriers;
            this.Extractor = extractor;
            this.Connectors = connectors;
            this.SchemaCache = schemaCache;
        }

        public IObjectSchemaExtractor GetExtractor()
        {
            return this.Extractor;
        }

        public ISchemaMigrator GetMigrator()
        {
            return this.Migrator;
        }

        public ISchemaManager GetManager(BackendType backendType)
        {
            var manager = this.Managers.SingleOrDefault(m => m.Backend == backendType);
            if (manager == null)
            {
                throw new NotImplementedException($"No manager backend type  for {backendType}");
            }
            return new CachedSchemaManager(this.SchemaCache, manager);
        }

        public ISchemaQuerier GetQuerier(BackendType backendType)
        {
            var querier = this.Queriers.SingleOrDefault(m => m.Backend == backendType);
            if (querier == null)
            {
                throw new NotImplementedException($"No querier for backend type {backendType}");
            }
            return querier;
        }

        public IConnectionFactory GetConnectionFactory(BackendType backendType)
        {
            var connector = this.Connectors.SingleOrDefault(m => m.Backend == backendType);
            if (connector == null)
            {
                throw new NotImplementedException($"No connection factory for backend type {backendType}");
            }
            return connector;
        }
    }
}
