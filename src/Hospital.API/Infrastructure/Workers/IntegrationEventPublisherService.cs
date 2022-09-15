using Hospital.API.Domain.Entities;
using Hospital.API.Infrastructure.Context;
using RabbitMQ.Client;
using System.Text;

namespace Hospital.API.Infrastructure.Workers;

public class IntegrationEventPublisherService : BackgroundService
{
    private IServiceProvider _serviceProvider;

    private const int DELAY_SECONDS = 3;

    private IConnectionFactory _factory;

    private readonly ILogger<IntegrationEventPublisherService> _logger;

    private IModel _channel;

    public IntegrationEventPublisherService(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventPublisherService> logger,
        ConnectionFactory factory)
    {
        _factory = factory;
        _channel = _factory.CreateConnection()
            .CreateModel();
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reading Events to publish");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(DELAY_SECONDS);
            await PublishIntegrationEvents();
        }

        _channel.Close();
    }

    private async Task PublishIntegrationEvents()
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<Event> events = dbContext.Events.Where(x => x.ProcessedDate == null)
                .AsQueryable()
                .ToList() ?? new();

            if (events is not null || events!.Any())
            {
                foreach (var @event in events!)
                {
                    AddEventToQueue(_channel, @event);
                }

                _logger.LogInformation($"Updating events processed Date");
                await dbContext.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            _channel.Close();
            _logger.LogError(ex.ToString());
        }
    }

    private void AddEventToQueue(IModel channel, Event @event)
    {
        channel.QueueDeclare(
            @event.EventName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        //Using default exchange
        channel.BasicPublish(
            exchange: "",
            routingKey: @event.EventName,
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(@event.Data)
            );

        @event.Processed();

        _logger.LogInformation($"Event published at {DateTime.UtcNow}");
    }
}
