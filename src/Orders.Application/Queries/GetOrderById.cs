using MediatR;
using Orders.Application.Dtos.ViewModels;
using Orders.Core.Repositories;

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

    public class GetOrderByIdHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderById, OrderViewModel>
    {
        public async Task<OrderViewModel> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetById(request.Id);
            var response = OrderViewModel.FromEntity(order);
            return response;

        }
    }
}
