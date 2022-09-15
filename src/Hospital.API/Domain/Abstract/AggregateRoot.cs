using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.API.Domain.Abstract
{
    public abstract class AggregateRoot : Entity
    {
        [NotMapped]
        public List<IDomainEvent> DomainEvents { get; protected set; }

        protected AggregateRoot() : base()
        {
        }

        protected AggregateRoot(Guid id) : base(id)
        {
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (DomainEvents is null)
            {
                DomainEvents = new List<IDomainEvent>();
            }

            DomainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            DomainEvents?.Clear();
        }
    }
}
