namespace Orders.Infrastructure.ServiceDiscovery
{
    public interface IServiceDiscovery
    {
        Task<Uri> GetServiceDiscoveryUriAsync(string serviceName, string requestUrl);
    }
}
