namespace Orders.Infrastructure.MessageBus
{
    public interface IMessageBusClient
    {
        void Pubish(object message,string routingKey,string exchange);

    }
}
