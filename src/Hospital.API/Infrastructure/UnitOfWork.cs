using Hospital.API.Domain.Abstract;
using Hospital.API.Domain.Entities;
using Hospital.API.Domain.Repositories;
using MediatR;
using Sales.API.Domain.Entities;

namespace Hospital.API.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IEventMapper _eventMapper;

    private readonly IMediator _mediator;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IEventMapper eventMapper,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _eventMapper = eventMapper;
        _mediator = mediator;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        List<Patient> patients = _dbContext.ChangeTracker.Entries<Patient>()
            .Where(x => x.Entity.DomainEvents is not null && x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        await _dbContext.SaveChangesAsync(cancellationToken);

        DispatchEvents(patients);
        ClearEvents(patients);
    }

    private async void DispatchEvents(List<Patient> patients) 
    {
        IEnumerable<IDomainEvent> domainEvents = patients.SelectMany(x => x.DomainEvents);

        foreach (var domainEvent in domainEvents) 
        {
            IIntegrationEvent? integrationEvent = _eventMapper.MapDomainEvent(domainEvent);

            if (integrationEvent is null) { continue; }

            await _mediator.Publish(
                new Event(Guid.NewGuid(), typeof(IIntegrationEvent).Name, DateTime.UtcNow, string.Empty)
                );
        }
    }

    private void ClearEvents(List<Patient> patients) 
    {
        patients.ForEach(x => x.ClearDomainEvents());
    }
}
