using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Repositories;
using Sales.API.Infrastructure.EventBus;

namespace Sales.API.Infrastructure;

public static class InjectDependencies
{
    public static void AddInfrastructure(this IServiceCollection services) 
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SalesDB");
        });

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddSingleton<IEventBusSubscriptions, EventBusSubscriptions>();
        services.AddSingleton<IEventBus, RabbitMQBus>();
        //TODO: rabbit MQ
    }
}
