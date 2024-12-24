using MongoDB.Driver;
using Orders.Core.Entitites;
using Orders.Core.Repositories;

namespace Orders.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>("orders");
        }

        public async Task Add(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        public async Task<Order> GetById(Guid id)
        {
            return await _orders.Find(c=>c.Id == id).SingleOrDefaultAsync();
        }

        public async Task Update(Order order)
        {
            await _orders.ReplaceOneAsync(f => f.Id == order.Id, order);
        }
    }
}
