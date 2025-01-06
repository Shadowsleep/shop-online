using Notifications.Api.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notifications.Api.Consumers
{
    public class NotificatePaymentAcceptedConsumer : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        private readonly string queueNameConsumer;
        public NotificatePaymentAcceptedConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
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

            connection = connectionFactory.CreateConnection("order-service-producer");
            channel = connection.CreateModel();

            queueNameConsumer = configuration["rabbitmq:payment-confirmed:queue"];
            var exchangeConsumer = configuration["rabbitmq:payment-confirmed:exchange"];
            var routingkeyConsumer = configuration["rabbitmq:payment-confirmed:routingkey"];
            channel.ExchangeDeclare(exchange: exchangeConsumer, type: ExchangeType.Topic, durable: true);
            channel.QueueDeclare(queue: queueNameConsumer, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueNameConsumer, exchange: exchangeConsumer, routingKey: routingkeyConsumer);

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();
                var orderCreatedMessageJson = Encoding.UTF8.GetString(byteArray);

                var orderDetails = JsonSerializer.Deserialize<OrderDto>(orderCreatedMessageJson);

                NotificateOrderCreated(orderDetails);
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume(queueNameConsumer, false, consumer);

            return Task.CompletedTask;
        }

        private void NotificateOrderCreated(OrderDto paymentInfo)
        {
            Console.WriteLine("Notificate payment Done");
        }
    }
}
