using Hospital.API.Domain.Abstract;
using Hospital.API.Domain.Entities;
using Hospital.API.Domain.Events;
using System.Text.Json;

namespace Hospital.API.Infrastructure;

public interface IEventMapper
{
    public IIntegrationEvent? MapDomainEvent<T>(T domainEvent) where T : IDomainEvent;

    public List<Event> Map(List<IDomainEvent> domainEvents);
}

public class EventMapper : IEventMapper
{
    public List<Event> Map(List<IDomainEvent> domainEvents)
    {
        List<IIntegrationEvent?> integrationEvents = domainEvents.Select(x => MapDomainEvent(x))
            .Where(x => x is not null)
            .ToList();

        return integrationEvents.Select(x => MapIntegrationEvent(x)).ToList();
    }

    public IIntegrationEvent? MapDomainEvent<T>(T domainEvent) where T : IDomainEvent
    {
        return domainEvent switch
        {
            PatientCreatedDomainEvent @event => new PatientCreatedIntegrationEvent(@event.Id, @event.Name, @event.Email, @event.Address),
            _ => null
        };
    }

    private Event MapIntegrationEvent(IIntegrationEvent integrationEvent) 
    {
        return new Event(
            Guid.NewGuid(),
            integrationEvent.GetType().Name,
            DateTime.UtcNow,
            JsonSerializer.Serialize(integrationEvent)
            );
    }
}
