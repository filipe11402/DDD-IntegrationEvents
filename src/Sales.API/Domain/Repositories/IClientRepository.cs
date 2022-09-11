using Sales.API.Domain.Entities;

namespace Sales.API.Domain.Repositories;

public interface IClientRepository
{
    Task<Client> Add(Client client);

    Task<Client> GetById(Guid id);
}
