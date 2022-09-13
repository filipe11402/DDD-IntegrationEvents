using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sales.API.Domain.Events;
using System.Text;

namespace Sales.API.Infrastructure.EventBus;

public class RabbitMQBus : IEventBus
{
    private readonly IEventBusSubscriptions _subscriptions;

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<RabbitMQBus> _logger;

    private IModel _channel;

    private IConnectionFactory _connection;

    public RabbitMQBus(
        IEventBusSubscriptions eventBusSubscriptions,
        IServiceProvider serviceProvider,
        ILogger<RabbitMQBus> logger)
    {
        _subscriptions = eventBusSubscriptions;
        _serviceProvider = serviceProvider;
        _connection = new ConnectionFactory();
        _channel = _connection.CreateConnection()
            .CreateModel();
        _logger = logger;
    }

    public Task Publish<TEvent>(TEvent @event) where TEvent : IIntegrationEvent
    {
        //TODO: publish via this service
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

        var consumer = new AsyncEventingBasicConsumer(_channel);

        //TODO: extract
        consumer.Received += Consumer_Received;

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer
            );

        _logger.LogInformation($"Subscribed to {queueName}");

        return Task.CompletedTask;
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs args) 
    {
        var queueName = args.RoutingKey;

        string queueMessageBodyAsString = Encoding.UTF8.GetString(args.Body.Span);

        Type? eventType = _subscriptions.GetEventType(queueName)!;

        IIntegrationEvent @event = (IIntegrationEvent)JsonConvert.DeserializeObject(queueMessageBodyAsString, eventType)!;

        await _subscriptions.HandleAsync(@event);

        _logger.LogInformation($"Event was handled at {DateTime.UtcNow}");

        _channel.BasicAck(args.DeliveryTag, multiple: false);

        _logger.LogInformation("Event was acknowledged");
    }

    public Task Unsubscribe<TEvent, TEventHandler>(TEvent @event)
        where TEvent : IIntegrationEvent
        where TEventHandler : INotificationHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}
