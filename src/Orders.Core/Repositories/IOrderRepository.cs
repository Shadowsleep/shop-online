using Orders.Core.Entitites;

namespace Orders.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetById(Guid id);
        Task Add(Order order);
        Task Update(Order order);
    }
}
