using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;

namespace Hospital.API.Infrastructure;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PatientRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Patient> Add(Patient client)
    {
        await _dbContext.Patients.AddAsync(client);
        await _dbContext.SaveChangesAsync();

        return client;
    }

    public async Task<Patient> GetById(Guid id)
    {
        return await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);
    }
}
