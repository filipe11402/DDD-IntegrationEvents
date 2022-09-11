using Sales.API.Domain.Entities;

namespace Sales.API.Domain.Repositories;

public interface IPatientRepository
{
    Task<Patient> Add(Patient client);

    Task<Patient> GetById(Guid id);
}
