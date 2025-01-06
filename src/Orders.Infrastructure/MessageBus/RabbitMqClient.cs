using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Orders.Infrastructure.MessageBus
{
    public class RabbitMqClient(IConnection connection) : IMessageBusClient
    {
        public void Pubish(object message, string routingKey, string exchange)
        {
            var channel = connection.CreateModel();
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
