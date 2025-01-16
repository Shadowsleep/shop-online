using MediatR;
using Orders.Application.Dtos.ViewModels;
using Orders.Core.Repositories;
using Orders.Infrastructure.CacheStorage;

namespace Orders.Application.Queries
{
    public class GetOrderById : IRequest<OrderViewModel>
    {
        public GetOrderById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class GetOrderByIdHandler(IOrderRepository orderRepository, ICacheService cacheService) : IRequestHandler<GetOrderById, OrderViewModel>
    {
        public async Task<OrderViewModel> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            var cacheKey = request.Id.ToString();
            var order = await cacheService.GetAsync<OrderViewModel>(cacheKey);
            if (order == null)
            {
                var entity = await orderRepository.GetById(request.Id);
                order = OrderViewModel.FromEntity(entity);
                await cacheService.SetAsync(cacheKey, order);
            }

            return order;

        }
    }
}
