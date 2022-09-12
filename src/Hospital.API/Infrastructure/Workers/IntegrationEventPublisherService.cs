using Hospital.API.Domain.Entities;
using Hospital.API.Infrastructure.Context;
using Hospital.API.Infrastructure.Mappers;
using RabbitMQ.Client;
using System.Text;

namespace Hospital.API.Infrastructure.Workers;

public class IntegrationEventPublisherService : BackgroundService
{
    private IServiceProvider _serviceProvider;

    private const int DELAY_SECONDS = 3;

    private ConnectionFactory _factory;

    private readonly ILogger<IntegrationEventPublisherService> _logger;

    public IntegrationEventPublisherService(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventPublisherService> logger)
    {
        _factory = new ConnectionFactory();
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("IntegrationEvents Worker is starting");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(DELAY_SECONDS);
            await PublishIntegrationEvents();
        }
    }

    private async Task PublishIntegrationEvents()
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<Event> events = dbContext.Events.AsQueryable().ToList() ?? new();

            if (events is not null || events!.Any())
            {
                IConnection connection = _factory.CreateConnection();

                using IModel channel = connection.CreateModel();

                foreach (var integrationEvent in events!)
                {
                    AddEventToQueue(channel, integrationEvent);
                }

                _logger.LogInformation($"Events that were published are being removed");
                dbContext.Events.RemoveRange(events!);
                await dbContext.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    private void AddEventToQueue(IModel channel, Event integrationEvent)
    {
        _logger.LogInformation($"Event published at {DateTime.UtcNow}");

        channel.QueueDeclare(
            integrationEvent.EventName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        //Using defauçt exchange
        channel.BasicPublish(
            exchange: "",
            routingKey: integrationEvent.EventName,
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(integrationEvent.Data)
            );
    }
}
