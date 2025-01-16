using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Core.Repositories;
using Orders.Infrastructure.MessageBus;
using Orders.Infrastructure.Repositories;
using Orders.Infrastructure.ServiceDiscovery;
using RabbitMQ.Client;

namespace Orders.Infrastructure
{
    public static class Configuration
    {
        public static void ConfigurationServicesOrderInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddTransient<IServiceDiscovery, ConsulService>();
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

        public static void ConfigurationConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p =>
                new ConsulClient(con =>
                {
                    var address = configuration["consul:host"];
                    con.Address = new Uri(address);
                })
            );
        }

        public static void UseConsul(this IApplicationBuilder builder)
        {
            var consulClient = builder.ApplicationServices.GetRequiredService<IConsulClient>();
            var lifetime = builder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var registration = new AgentServiceRegistration()
            {
                ID = "orders-service",
                Name = "Orders-Service",
                Address = "localhost",
                Port = 6000
            };

            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(async () =>
            {
                Console.WriteLine("Deregistering from Consul");
                await consulClient.Agent.ServiceDeregister(registration.ID);
            });
        }
    }
}
