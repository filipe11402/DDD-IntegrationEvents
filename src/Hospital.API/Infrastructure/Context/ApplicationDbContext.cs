using Hospital.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Entities;

namespace Hospital.API.Infrastructure.Context;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
