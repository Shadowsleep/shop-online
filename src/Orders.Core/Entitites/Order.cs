using Orders.Core.Enums;
using Orders.Core.Events;
using Orders.Core.ValueObjects;

namespace Orders.Core.Entitites
{
    public class Order : AggragateRoot
    {
        public Order(Customer customer, DeliveryAddress deliveryAddress, PaymentAddress paymentAddress, PaymentInfo paymentInfo, List<OrderItem> items)
        {
            Id = Guid.NewGuid();
            Customer = customer;
            DeliveryAddress = deliveryAddress;
            PaymentAddress = paymentAddress;
            PaymentInfo = paymentInfo;
            Items = items;

            TotalPrice = Items.Sum(x => x.Quantity * x.Price);
            CreatedAt = DateTime.Now;
            Status = OrderStatus.Created;

            AddEvent(new OrderCreated(Id, TotalPrice, paymentInfo, customer.FullName, customer.Email));
        }

        public decimal TotalPrice { get; private set; }
        public Customer Customer { get; private set; }
        public DeliveryAddress DeliveryAddress { get; private set; }
        public PaymentAddress PaymentAddress { get; private set; }
        public PaymentInfo PaymentInfo { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public OrderStatus Status { get; private set; }

        public void SetSetAsCompleted() => Status = OrderStatus.Completed;
        public void SetSetAsReject() => Status = OrderStatus.Rejected;

    }
}
