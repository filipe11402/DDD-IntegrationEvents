using MediatR;
using Sales.API.Domain.Events;

namespace Sales.API.Infrastructure.EventBus;

public interface IEventBus
{
    Task Subscribe<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>;

    Task Publish<TEvent> (TEvent @event)
        where TEvent : IIntegrationEvent;

    Task Unsubscribe<TEvent, TEventHandler> (TEvent @event)
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>;
}
