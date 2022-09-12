using Hospital.API.Domain.Abstract;
using Hospital.API.Domain.Entities;
using Hospital.API.Domain.Events;

namespace Hospital.API.Infrastructure.Mappers;

public interface IEventMapper
{
    public IIntegrationEvent? MapDomainEvent<T>(T domainEvent) where T : IDomainEvent;
}

public class EventMapper : IEventMapper
{
    public IIntegrationEvent? MapDomainEvent<T>(T domainEvent) where T : IDomainEvent
    {
        return domainEvent switch
        {
            PatientCreatedDomainEvent @event => new PatientCreatedIntegrationEvent(@event.Id, @event.Name, @event.Email, @event.Address),
            _ => null
        };
    }
}
