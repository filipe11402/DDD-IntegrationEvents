using Hospital.API.Domain.Repositories;
using Hospital.API.Infrastructure.Context;
using Hospital.API.Infrastructure.Mappers;
using Hospital.API.Infrastructure.Repositories;
using Hospital.API.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Repositories;

namespace Hospital.API.Infrastructure;

public static class InjectDependencies
{
    public static void AddInfrastructure(this IServiceCollection services) 
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SalesDB");
        });

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IEventMapper, EventMapper>();
        services.AddHostedService<IntegrationEventPublisherService>();
        //TODO: rabbit MQ
    }
}
