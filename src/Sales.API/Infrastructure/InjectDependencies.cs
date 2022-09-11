using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Repositories;

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
        //TODO: rabbit MQ
    }
}
