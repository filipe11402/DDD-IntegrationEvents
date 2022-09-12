using MediatR;

namespace Sales.API.Infrastructure;

public interface IEventBusSubscriptions
{
    Task AddSubscription(IIntegrationEvent @event, INotificationHandler<IIntegrationEvent> eventHandler);

    Task RemoveSubscription(string @event);
}

public class EventBusSubscriptions : IEventBusSubscriptions
{
    private Dictionary<string, INotificationHandler<IIntegrationEvent>> _handlers = new();

    public EventBusSubscriptions()
    {

    }

    public Task AddSubscription(IIntegrationEvent @event, INotificationHandler<IIntegrationEvent> eventHandler)
    {
        if (!_handlers.ContainsKey(@event.GetType().Name)) 
        {
            _handlers.Add(@event.GetType().Name, eventHandler);
        }

        return Task.CompletedTask;
    }

    public Task RemoveSubscription(string @event)
    {
        if (_handlers.ContainsKey(@event)) 
        {
            _handlers.Remove(@event);
        }

        return Task.CompletedTask;
    }
}
