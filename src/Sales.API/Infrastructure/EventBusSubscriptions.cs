using MediatR;
using Sales.API.Domain.Events;

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

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<EventBusSubscriptions> _logger;

    public EventBusSubscriptions(
        IServiceProvider serviceProvider,
        ILogger<EventBusSubscriptions> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
        KeyValuePair<Type, Type> subscribedEvent = _handlers.FirstOrDefault(x => x.Key.Name == @event.GetType().Name);

        if (subscribedEvent.Equals(default(KeyValuePair<Type, Type>)))
        {
            _logger.LogInformation($"No subscribed event was found for {@event.GetType().Name}");
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var eventHandler = scope.ServiceProvider.GetService(subscribedEvent.Value);

        if (eventHandler is null)
        {
            _logger.LogInformation($"No Registered handler was found for {subscribedEvent.Value.Name}");
            return;
        }

        await (Task)subscribedEvent.Value.GetMethod("Handle")!.Invoke(eventHandler, new object[] { @event, CancellationToken.None })!;
    }

    public Task RemoveSubscription(string @event)
    {
        return Task.CompletedTask;
    }
}
