using Microsoft.EntityFrameworkCore;
using Sales.API.Domain.Entities;

namespace Sales.API.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Client> Clients { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
