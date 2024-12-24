using Microsoft.Extensions.DependencyInjection;
using Orders.Core.Repositories;
using Orders.Infrastructure.Repositories;

namespace Orders.Infrastructure
{
    public static class Configuration
    {
        public static void ConfigurationServicesOrderInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
