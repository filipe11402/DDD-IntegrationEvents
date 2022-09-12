using Hospital.API.Domain.Abstract;
using Hospital.API.Domain.Entities;
using Hospital.API.Domain.Repositories;
using Hospital.API.Infrastructure.Context;
using Hospital.API.Infrastructure.Mappers;
using MediatR;
using Newtonsoft.Json;
using Sales.API.Domain.Entities;

namespace Hospital.API.Infrastructure.Repositories;

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

    //TODO: use base entity class
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

            //Dispatch the event via Mediator as it is a Domain event
            if (integrationEvent is null) { continue; }

            await _dbContext.Events.AddAsync(
                new Event(Guid.NewGuid(),
                integrationEvent.GetType().Name,
                DateTime.UtcNow,
                JsonConvert.SerializeObject(integrationEvent)
                ));

            await _dbContext.SaveChangesAsync();
        }
    }

    private void ClearEvents(List<Patient> patients)
    {
        patients.ForEach(x => x.ClearDomainEvents());
    }
}
