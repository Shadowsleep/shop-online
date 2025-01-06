
using Payments.Api.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Payments.Api.Consumers
{
    public class ProcessPaymentConsumer : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        private readonly string queueNameConsumer;
        private readonly string exchangePubish;
        private readonly string routingkeyPubish;
        public ProcessPaymentConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
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

            queueNameConsumer = configuration["rabbitmq:order-created:queue"];
            DeclareConsumer(configuration);

            exchangePubish = configuration["rabbitmq:payment-confirmed:exchange"];
            routingkeyPubish = configuration["rabbitmq:payment-confirmed:routingkey"];
            DeclarePublisher();

        }

        private void DeclarePublisher()
        {
            channel.ExchangeDeclare(exchange: exchangePubish, type: ExchangeType.Topic, durable: true);
        }

        private void DeclareConsumer(IConfiguration configuration)
        {
            var exchangeConsumer = configuration["rabbitmq:order-created:exchange"];
            var routingkeyConsumer = configuration["rabbitmq:order-created:routingkey"];
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
                var orderCreatedJson = Encoding.UTF8.GetString(byteArray);

                var orderInfo = JsonSerializer.Deserialize<OrderDto>(orderCreatedJson);

                ProcessPayment(orderInfo.PaymentInfo);
                CreateEventPaymentAproved(orderInfo.Id);
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume(queueNameConsumer, false, consumer);

            return Task.CompletedTask;
        }

        private void ProcessPayment(PaymentInfo paymentInfo)
        {
            Console.WriteLine("Payment processed");
        }

        private void CreateEventPaymentAproved(Guid id)
        {
            var paymentAproved = new { Id = id };
            var json = JsonSerializer.Serialize(paymentAproved);
            var bytes = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: exchangePubish,
                routingKey: routingkeyPubish,
                basicProperties: null,
                body: bytes
                );
            Console.WriteLine($"publish in exchange {exchangePubish}");
        }
    }
}
