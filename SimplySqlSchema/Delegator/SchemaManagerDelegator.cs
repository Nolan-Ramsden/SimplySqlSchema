using System;
using System.Linq;
using System.Collections.Generic;
using SimplySqlSchema.Cache;

namespace SimplySqlSchema.Delegator
{
    public class SchemaManagerDelegator : ISchemaManagerDelegator
    {
        protected ISchemaCache SchemaCache { get; }
        protected IObjectSchemaExtractor Extractor { get; }
        protected IEnumerable<ISchemaQuerier> Queriers { get; }
        protected IEnumerable<ISchemaManager> Managers { get; }
        
        public SchemaManagerDelegator(ISchemaCache schemaCache = null, IEnumerable<ISchemaManager> managers = null, IEnumerable<ISchemaQuerier> queriers = null, IObjectSchemaExtractor extractor = null)
        {
            this.Managers = managers;
            this.Queriers = queriers;
            this.Extractor = extractor;
            this.SchemaCache = schemaCache;
        }

        public ISchemaManager GetManager(BackendType backendType)
        {
            if (this.Managers == null)
            {
                throw new InvalidOperationException($"No SchemaManagers registered");
            }
            var manager = this.Managers.SingleOrDefault(m => m.Backend == backendType);
            if (manager == null)
            {
                throw new NotImplementedException($"Not backend type manager for {backendType}");
            }

            if (this.SchemaCache == null)
            {
                return manager;
            }
            else
            {
                return new CachedSchemaManager(
                    impl: manager,
                    schemaCache: this.SchemaCache
                );
            }
        }

        public ISchemaQuerier GetQuerier(BackendType backendType)
        {
            if (this.Queriers == null)
            {
                throw new InvalidOperationException($"No SchemaQueriers registered");
            }
            var querier = this.Queriers.SingleOrDefault(m => m.Backend == backendType);
            if (querier == null)
            {
                throw new NotImplementedException($"Not backend type querier for {backendType}");
            }
            return querier;
        }

        public IObjectSchemaExtractor GetSchemaExtractor()
        {
            if (this.Extractor == null)
            {
                throw new InvalidOperationException($"No SchemaExtractor registered");
            }

            if (this.SchemaCache == null)
            {
                return this.Extractor;
            }
            else
            {
                return new CachedObjectSchemaExtractor(
                    impl: this.Extractor,
                    schemaCache: this.SchemaCache
                );
            }
        }
    }
}
