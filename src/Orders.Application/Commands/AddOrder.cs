using MediatR;
using Orders.Core.Entitites;
using Orders.Core.Repositories;
using Orders.Core.ValueObjects;
using Orders.Infrastructure.MessageBus;

namespace Orders.Application.Commands
{
    public class AddOrder : IRequest<Guid>
    {
        public CustomerInputModel Customer { get; set; }
        public DeliveryAddressInputModel DeliveryAddress { get; set; }
        public PaymentAddressInputModel PaymentAddress { get; set; }
        public PaymentInfoInputModel PaymentInfo { get; set; }

        public List<OrderItemInputModel> Items { get; set; }

        public Order ToEntity()
        {
            return new Order(
                customer: CustomerInputModel.ToEntity(Customer),
                deliveryAddress: DeliveryAddressInputModel.ToEntity(DeliveryAddress),
                paymentAddress: PaymentAddressInputModel.ToEntity(PaymentAddress),
                paymentInfo: PaymentInfoInputModel.ToEntity(PaymentInfo),
                items: Items.Select(x => OrderItemInputModel.ToEntity(x)).ToList()
                );
        }
    }

    public class CustomerInputModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public static Customer ToEntity(CustomerInputModel customer)
        {
            return new Customer(customer.Id, customer.FullName, customer.Email);
        }
    }

    public class DeliveryAddressInputModel
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public static DeliveryAddress ToEntity(DeliveryAddressInputModel deliveryAddress)
        {
            return new DeliveryAddress(deliveryAddress.Street, deliveryAddress.Number, deliveryAddress.City, deliveryAddress.State, deliveryAddress.ZipCode);
        }
    }

    public class PaymentAddressInputModel
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public static PaymentAddress ToEntity(PaymentAddressInputModel paymentAddress)
        {
            return new PaymentAddress(paymentAddress.Street, paymentAddress.Number, paymentAddress.City, paymentAddress.Street, paymentAddress.ZipCode);
        }
    }

    public class PaymentInfoInputModel
    {
        public string CardNumber { get; set; }
        public string FullName { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public static PaymentInfo ToEntity(PaymentInfoInputModel paymentInfo)
        {
            return new PaymentInfo(paymentInfo.CardNumber, paymentInfo.FullName, paymentInfo.Expiration, paymentInfo.CVV);
        }
    }

    public class OrderItemInputModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public static OrderItem ToEntity(OrderItemInputModel orderItem)
        {
            return new OrderItem(orderItem.ProductId, orderItem.Quantity, orderItem.Price);
        }
    }


    public class AddOrderHandler(IOrderRepository repository, IMessageBusClient messageBus) : IRequestHandler<AddOrder, Guid>
    {
        private const string Exchange = "order-service";
        private const string RoutingKey = "order-created";

        public async Task<Guid> Handle(AddOrder request, CancellationToken cancellationToken)
        {
            var order = request.ToEntity();
            await repository.Add(order);

            foreach(var item in order.Events)
            {
                messageBus.Pubish(item, RoutingKey, Exchange);
            }
            return order.Id;
        }
    }
}