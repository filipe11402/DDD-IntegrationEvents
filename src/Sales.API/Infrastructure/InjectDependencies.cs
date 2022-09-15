using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Sales.API.Domain.Repositories;
using Sales.API.Infrastructure.Context;
using Sales.API.Infrastructure.EventBus;
using Sales.API.Infrastructure.Events;
using Sales.API.Infrastructure.Events.Handlers;
using Sales.API.Infrastructure.Repositories;
using Sales.API.Infrastructure.Subscriptions;

namespace Sales.API.Infrastructure;

public static class InjectDependencies
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SalesDB");
        });
        services.AddTransient<PatientCreatedIntegrationEventHandler>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddSingleton<IEventBusSubscriptions, EventBusSubscriptions>();
        services.AddSingleton<IEventBus, RabbitMQBus>();

        services.AddSingleton(
            new ConnectionFactory { 
                Uri = new Uri(configuration.GetSection("BusConnection").Value)
            });
    }

    public static void RegisterEventSubscriptions(this IServiceProvider serviceProvider) 
    {
        using var scope = serviceProvider.CreateScope();

        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        eventBus.Subscribe<PatientCreatedIntegrationEvent, PatientCreatedIntegrationEventHandler>();
    }
}
