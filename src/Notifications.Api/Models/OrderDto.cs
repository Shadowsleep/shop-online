namespace Notifications.Api.Models
{
    public record OrderDto(Guid Id, decimal TotalPrice, PaymentInfo PaymentInfo, string FullName, string Email);

    public struct PaymentInfo
    {
        public string CardNumber { get; private set; }
        public string FullName { get; private set; }
        public string Expiration { get; private set; }
        public string CVV { get; private set; }
    }
}
