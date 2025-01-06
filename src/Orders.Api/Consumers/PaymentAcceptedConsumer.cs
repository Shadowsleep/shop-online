
using Orders.Api.Dtos;
using Orders.Core.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Orders.Api.Consumers
{
    public class PaymentAcceptedConsumer : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        private readonly string queueNameConsumer;
        private readonly IServiceProvider _service;
        private ILogger<PaymentAcceptedConsumer> _logger;

        public PaymentAcceptedConsumer(IServiceProvider service, IConfiguration configuration,ILogger<PaymentAcceptedConsumer> logger)
        {
            this._service = service;
            this._logger = logger;
            var hostName = configuration["rabbitmq:host"];
            var port = int.Parse(configuration["rabbitmq:port"] ?? "5672");
            var userName = configuration["rabbitmq:username"];
            var password = configuration["rabbitmq:password"];
            var exchangeConsumer = configuration["rabbitmq:payment-confirmed:exchange"];
            var routingkeyConsumer = configuration["rabbitmq:payment-confirmed:routingkey"];
            queueNameConsumer = configuration["rabbitmq:payment-confirmed:queue"];

            var connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };

            connection = connectionFactory.CreateConnection("order-service-producer");

            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeConsumer, type: ExchangeType.Topic, durable: true);
            channel.QueueDeclare(queue: queueNameConsumer, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueNameConsumer, exchange: exchangeConsumer, routingKey: routingkeyConsumer);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();
                var orderCreatedMessageJson = Encoding.UTF8.GetString(byteArray);
                var orderDetails = JsonSerializer.Deserialize<PaymentAccepted>(orderCreatedMessageJson);
                var result = await UpdateOrder(orderDetails);
                if (result)
                    channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            channel.BasicConsume(queueNameConsumer, false, consumer);
            return Task.CompletedTask;

        }

        private async Task<bool> UpdateOrder(PaymentAccepted orderDetails)
        {
            try
            {
                using var scope = _service.CreateScope();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var order = await orderRepository.GetById(orderDetails.Id);
                order.SetSetAsCompleted();
                await orderRepository.Update(order);
                _logger.LogInformation("Updated order");
                return true;
            }
            catch
            {
                _logger.LogError("Error updating order");
                return false;
            }
            throw new NotImplementedException();
        }
    }
}
