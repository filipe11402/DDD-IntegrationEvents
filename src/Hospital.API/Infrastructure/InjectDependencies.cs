using Hospital.API.Domain.Repositories;
using Hospital.API.Infrastructure.Context;
using Hospital.API.Infrastructure.Mappers;
using Hospital.API.Infrastructure.Repositories;
using Hospital.API.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Sales.API.Domain.Repositories;

namespace Hospital.API.Infrastructure;

public static class InjectDependencies
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("HospitalDB");
        });

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IEventMapper, EventMapper>();
        services.AddHostedService<IntegrationEventPublisherService>();

        services.AddSingleton(
            new ConnectionFactory
            {
                Uri = new Uri(configuration.GetSection("BusConnection").Value)
            });
    }
}
