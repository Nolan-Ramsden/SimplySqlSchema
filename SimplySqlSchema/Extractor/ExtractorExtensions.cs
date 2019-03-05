using Microsoft.Extensions.DependencyInjection;
using SimplySqlSchema.Extractor;

namespace SimplySqlSchema
{
    public static class ExtractorExtensions
    {
        public static IServiceCollection AddDataAnnotationExtractor(this IServiceCollection services)
        {
            return services.AddScoped<IObjectSchemaExtractor, DataAnnotationsSchemaExtractor>();
        }
    }
}
