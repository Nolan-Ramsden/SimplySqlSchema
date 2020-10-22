using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Delegator;

namespace SimplySqlSchema
{
    public static class DelegatorExtensions
    {
        public static IServiceCollection AddDelegator(this IServiceCollection services)
        {
            return services.AddScoped<ISimplySqlSchema, ProviderDelegator>();
        }
    }
}
