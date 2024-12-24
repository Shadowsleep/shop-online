using Orders.Core.Events;

namespace Orders.Core.Entitites
{
    public class AggragateRoot : IEntityBase
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public Guid Id { get; protected set; }

        public IEnumerable<IDomainEvent> Events => _events;

        protected void AddEvent(IDomainEvent e)
        {
            _events.Add(e);
        }
    }
}
