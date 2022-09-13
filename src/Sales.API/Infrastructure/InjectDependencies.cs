using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Repositories;
using Sales.API.Infrastructure.EventBus;
using Sales.API.Infrastructure.Events;
using Sales.API.Infrastructure.Events.Handlers;

namespace Sales.API.Infrastructure;

public static class InjectDependencies
{
    public static void AddInfrastructure(this IServiceCollection services) 
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SalesDB");
        });
        services.AddTransient<PatientCreatedIntegrationEventHandler>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddSingleton<IEventBusSubscriptions, EventBusSubscriptions>();
        services.AddSingleton<IEventBus, RabbitMQBus>();   
    }

    public static void RegisterEventSubscriptions(this IServiceProvider serviceProvider) 
    {
        using var scope = serviceProvider.CreateScope();

        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        eventBus.Subscribe<PatientCreatedIntegrationEvent, PatientCreatedIntegrationEventHandler>();
    }
}
