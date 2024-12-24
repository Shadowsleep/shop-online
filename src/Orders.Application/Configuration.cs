using Microsoft.Extensions.DependencyInjection;

namespace Orders.Application
{
    public static class Configuration
    {
        public static IServiceCollection ConfigurationServicesOrderApplication(this IServiceCollection services)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(Configuration).Assembly));
            return services;
        }
    }
}
