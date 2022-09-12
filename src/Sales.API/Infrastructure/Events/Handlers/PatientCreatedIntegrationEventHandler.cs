using MediatR;
using Sales.API.Domain.Repositories;

namespace Sales.API.Infrastructure.Events.Handlers;

public class PatientCreatedIntegrationEventHandler : INotificationHandler<PatientCreatedIntegrationEvent>
{
    private readonly IClientRepository _clientRepository;

    public PatientCreatedIntegrationEventHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public Task Handle(PatientCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
