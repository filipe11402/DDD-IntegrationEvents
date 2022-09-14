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

    private readonly ILogger<RabbitMQBus> _logger;

    private IModel _channel;

    private IConnectionFactory _connection;

    public RabbitMQBus(
        IEventBusSubscriptions eventBusSubscriptions,
        ILogger<RabbitMQBus> logger)
    {
        _subscriptions = eventBusSubscriptions;
        _connection = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@rabbitmq:5672/") };
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

        _channel = _connection.CreateConnection()
            .CreateModel();

        //_channel.QueueBind(
        //    queue: queueName,
        //    exchange: null,
        //    routingKey: queueName,
        //    arguments: null
        //    );

        _channel.QueueDeclare(
            queueName,
            true,
            false,
            false,
            arguments: null
            );

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += Consumer_Received;

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer
            );

        _logger.LogInformation($"Subscribed to {queueName}");

        return Task.CompletedTask;
    }

    private void Consumer_Received(object sender, BasicDeliverEventArgs args) 
    {
        var queueName = args.RoutingKey;

        string queueMessageBodyAsString = Encoding.UTF8.GetString(args.Body.Span);

        Type? eventType = _subscriptions.GetEventType(queueName)!;

        IIntegrationEvent @event = (IIntegrationEvent)JsonConvert.DeserializeObject(queueMessageBodyAsString, eventType)!;

        Task.WaitAll(_subscriptions.HandleAsync(@event));

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
