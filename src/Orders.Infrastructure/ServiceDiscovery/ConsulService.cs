using Consul;

namespace Orders.Infrastructure.ServiceDiscovery
{
    public class ConsulService(IConsulClient _consulClient) : IServiceDiscovery
    {
        public async Task<Uri> GetServiceDiscoveryUriAsync(string serviceName, string requestUrl)
        {
            var services = await _consulClient.Catalog.Service(serviceName);
            if (services.Response.Length == 0)
            {
                throw new Exception($"Service '{serviceName}' does not exist.");
            }
            var service = services.Response.First();
            var uri = new Uri($"{service.ServiceAddress}:{service.ServicePort}/{requestUrl}");
            return uri;
        }
    }
}
