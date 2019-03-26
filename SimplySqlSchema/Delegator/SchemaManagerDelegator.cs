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
        
        public SchemaManagerDelegator(IEnumerable<ISchemaManager> managers = null, IEnumerable<ISchemaQuerier> queriers = null, IObjectSchemaExtractor extractor = null)
        {
            this.Managers = managers;
            this.Queriers = queriers;
            this.Extractor = extractor;
        }

        public ISchemaManager GetSchemaManager(BackendType backendType)
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

        public IObjectSchemaExtractor GetSchemaExtractor()
        {
            if (this.Extractor == null)
            {
                throw new InvalidOperationException($"No SchemaExtractor registered");
            }

            return this.Extractor;
        }
    }
}
