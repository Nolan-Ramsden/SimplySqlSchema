using System;
using System.Linq;
using System.Collections.Generic;
using SimplySqlSchema.Cache;

namespace SimplySqlSchema.Delegator
{
    public class SchemaManagerDelegator : ISchemaManagerDelegator
    {
        protected IObjectSchemaExtractor Extractor { get; }
        protected IEnumerable<ISchemaQuerier> Queriers { get; }
        protected IEnumerable<ISchemaManager> Managers { get; }
        protected ISchemaCache SchemaCache { get; }
        
        public SchemaManagerDelegator(IEnumerable<ISchemaManager> managers = null, IEnumerable<ISchemaQuerier> queriers = null, IObjectSchemaExtractor extractor = null, ISchemaCache schemaCache = null)
        {
            this.Managers = managers;
            this.Queriers = queriers;
            this.Extractor = extractor;
            this.SchemaCache = schemaCache;
        }

        public ISchemaManager GetSchemaManager(BackendType backendType, bool cacheWrap = false)
        {
            if (this.Managers == null)
            {
                throw new InvalidOperationException($"No SchemaManagers registered");
            }
            var manager = this.Managers.SingleOrDefault(m => m.Backend == backendType);
            if (manager == null)
            {
                throw new NotImplementedException($"No manager backend type  for {backendType}");
            }

            if (cacheWrap)
            {
                return new CachedSchemaManager(this.SchemaCache, manager);
            }
            return manager;
        }

        public ISchemaQuerier GetSchemaQuerier(BackendType backendType)
        {
            if (this.Queriers == null)
            {
                throw new InvalidOperationException($"No SchemaQueriers registered");
            }
            var querier = this.Queriers.SingleOrDefault(m => m.Backend == backendType);
            if (querier == null)
            {
                throw new NotImplementedException($"No querier for backend type {backendType}");
            }
            return querier;
        }

        public IObjectSchemaExtractor GetSchemaExtractor(bool cacheWrap = false)
        {
            if (this.Extractor == null)
            {
                throw new InvalidOperationException($"No SchemaExtractor registered");
            }

            if (cacheWrap)
            {
                return new CachedObjectSchemaExtractor(this.SchemaCache, this.Extractor);
            }

            return this.Extractor;
        }
    }
}
