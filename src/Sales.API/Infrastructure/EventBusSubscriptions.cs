using MediatR;

namespace Sales.API.Infrastructure;

public interface IEventBusSubscriptions
{
    Task AddSubscription<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>;

    Task RemoveSubscription(string @event);

    Type? GetHandler(string @event);

    Type? GetEventType(string @event);

    Task HandleAsync<T>(T @event) where T : IIntegrationEvent;
}

public class EventBusSubscriptions : IEventBusSubscriptions
{
    private Dictionary<Type, Type> _handlers = new();

    private List<Type> _eventTypes = new();

    //private readonly 

    public EventBusSubscriptions()
    {
    }

    public Task AddSubscription<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>
    {
        if (!_handlers.ContainsKey(typeof(TEvent)))
        {   
            _handlers.Add(typeof(TEvent), typeof(TEventHandler));
            _eventTypes.Add(typeof(TEvent));
        }

        return Task.CompletedTask;
    }

    public Type? GetEventType(string @event)
    {
        return _handlers.FirstOrDefault(x => x.Key.Name == @event).Key;
    }

    public Type? GetHandler(string @event)
    {
        return _handlers.FirstOrDefault(token => token.Key.Name == @event).Value;
    }

    public async Task HandleAsync<T>(T @event) where T : IIntegrationEvent
    {
        var eventHandler = _handlers.GetValueOrDefault(@event.GetType());

        if (eventHandler is null) { return; } 
        
    }

    public Task RemoveSubscription(string @event)
    {
        //if (_handlers.ContainsKey(@event)) 
        //{
        //    _handlers.Remove(@event);
        //}

        return Task.CompletedTask;
    }
}
