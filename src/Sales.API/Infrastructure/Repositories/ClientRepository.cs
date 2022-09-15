using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;
using Sales.API.Infrastructure.Context;

namespace Sales.API.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ClientRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Client> Add(Client client)
    {
        await _dbContext.Clients.AddAsync(client);
        await _dbContext.SaveChangesAsync();

        return client;
    }

    public async Task<Client> GetById(Guid id)
    {
        return await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id);
    }
}
