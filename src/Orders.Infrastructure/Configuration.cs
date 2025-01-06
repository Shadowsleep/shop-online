using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Core.Repositories;
using Orders.Infrastructure.MessageBus;
using Orders.Infrastructure.Repositories;
using RabbitMQ.Client;

namespace Orders.Infrastructure
{
    public static class Configuration
    {
        public static void ConfigurationServicesOrderInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddSingleton<IMessageBusClient, RabbitMqClient>();

            var hostName = configuration["rabbitmq:host"];
            var port = int.Parse(configuration["rabbitmq:port"] ?? "5672");
            var userName = configuration["rabbitmq:username"];
            var password = configuration["rabbitmq:password"];
            var connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };

            var connection = connectionFactory.CreateConnection("order-service-producer");

            services.AddSingleton(connection);
        }

    }
}
