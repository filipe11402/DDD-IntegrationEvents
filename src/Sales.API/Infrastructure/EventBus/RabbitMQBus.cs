using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Sales.API.Infrastructure.EventBus;

public class RabbitMQBus : IEventBus
{
    private readonly IEventBusSubscriptions _subscriptions;

    private readonly IServiceProvider _serviceProvider;

    private IModel _channel;

    private IConnectionFactory _connection;

    public RabbitMQBus(
        IEventBusSubscriptions eventBusSubscriptions,
        IServiceProvider serviceProvider)
    {
        _subscriptions = eventBusSubscriptions;
        _serviceProvider = serviceProvider;
        _connection = new ConnectionFactory();
        _channel = _connection.CreateConnection()
            .CreateModel();
    }

    public Task Publish<TEvent>(TEvent @event) where TEvent : IIntegrationEvent
    {
        throw new NotImplementedException();
    }

    public Task Subscribe<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>
    {
        var queueName = typeof(TEvent).Name;
        var brokerName = "";

        _subscriptions.AddSubscription<TEvent, TEventHandler>();

        _channel.QueueDeclare(
            queueName,
            true,
            false,
            false,
            arguments: null
            );

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (ch, args) =>
        {
            string queueMessageBodyAsString = Encoding.UTF8.GetString(args.Body.Span);

            Type? eventType = _subscriptions.GetEventType(queueName)!;
            Type? eventHandlerType = _subscriptions.GetHandler(queueName)!;

            if (eventHandlerType is null) { Console.WriteLine("No event handler type was found for this queue"); return; }

            //TODO: extract no another class
            using var scope = _serviceProvider.CreateScope();

            var eventHandler = scope.ServiceProvider.GetService(eventHandlerType);

            if (eventHandler is null) { Console.WriteLine($"No handler found for {queueName} event"); return; }

            var @event = (IIntegrationEvent)JsonConvert.DeserializeObject(queueMessageBodyAsString, eventType)!;

            await (Task)eventHandlerType.GetMethod("Handle").Invoke(eventHandler, new object[] { @event, CancellationToken.None });

            //await eventHandler.Handle(@event as IIntegrationEvent, CancellationToken.None);

            Console.WriteLine($"Event was handled at {DateTime.UtcNow}");

            _channel.BasicAck(args.DeliveryTag, multiple: false);

            Console.WriteLine("Event was acknowledged");
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer
            );

        Console.WriteLine("Everything went OK");

        return Task.CompletedTask;
    }

    public Task Unsubscribe<TEvent, TEventHandler>(TEvent @event)
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}
