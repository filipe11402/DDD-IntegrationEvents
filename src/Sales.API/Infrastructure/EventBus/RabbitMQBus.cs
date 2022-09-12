using MediatR;

namespace Sales.API.Infrastructure.EventBus;

public class RabbitMQBus : IEventBus
{
    private Dictionary<IIntegrationEvent, List<INotificationHandler<IIntegrationEvent>>> _subscribers = new();

    public Task Publish<TEvent>(TEvent @event) where TEvent : IIntegrationEvent
    {
        throw new NotImplementedException();
    }

    public Task Subscribe<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>
    {
        throw new NotImplementedException();
    }

    public Task Unsubscribe<TEvent, TEventHandler>(TEvent @event)
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}
