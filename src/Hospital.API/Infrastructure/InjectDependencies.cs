using Hospital.API.Domain.Repositories;
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
        services.AddScoped<IEventMapper, EventMapper>();
        //TODO: rabbit MQ
    }
}
