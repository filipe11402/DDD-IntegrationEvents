using Hospital.API.Domain.Abstract;
using Hospital.API.Domain.Entities;
using Hospital.API.Domain.Repositories;
using Hospital.API.Infrastructure.Context;
using Hospital.API.Infrastructure.Mappers;
using MediatR;
using Newtonsoft.Json;

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

    public async Task Commit(CancellationToken cancellationToken)
    {
        List<AggregateRoot> patients = _dbContext.ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents is not null && x.Entity.DomainEvents!.Any())
            .Select(x => x.Entity)
            .ToList();

        await _dbContext.SaveChangesAsync(cancellationToken);

        DispatchEvents(patients);
        ClearEvents(patients);
    }

    private async void DispatchEvents(List<AggregateRoot> aggregates)
    {
        IEnumerable<IDomainEvent> domainEvents = aggregates.SelectMany(x => x.DomainEvents);

        foreach (var domainEvent in domainEvents)
        {
            IIntegrationEvent? integrationEvent = _eventMapper.MapDomainEvent(domainEvent);

            if (integrationEvent is null) { continue; }

            await _dbContext.Events.AddAsync(
                new Event(
                Guid.NewGuid(),
                integrationEvent.GetType().Name,
                DateTime.UtcNow,
                JsonConvert.SerializeObject(integrationEvent)
                ));

            await _dbContext.SaveChangesAsync();
        }
    }

    private void ClearEvents(List<AggregateRoot> aggregates)
    {
        aggregates.ForEach(x => x.ClearDomainEvents());
    }
}
