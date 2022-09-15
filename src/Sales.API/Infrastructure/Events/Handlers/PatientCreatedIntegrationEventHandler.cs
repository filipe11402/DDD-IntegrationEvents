using MediatR;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;

namespace Sales.API.Infrastructure.Events.Handlers;

public class PatientCreatedIntegrationEventHandler : INotificationHandler<PatientCreatedIntegrationEvent>
{
    private readonly IClientRepository _clientRepository;

    private readonly ILogger<PatientCreatedIntegrationEventHandler> _logger;

    public PatientCreatedIntegrationEventHandler(
        IClientRepository clientRepository,
        ILogger<PatientCreatedIntegrationEventHandler> logger)
    {
        _clientRepository = clientRepository;
        _logger = logger;
    }

    public async Task Handle(PatientCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var client = new Client(
            notification.Id,
            notification.Name,
            notification.Email,
            notification.Address
            );

        await _clientRepository.Add(client);
    }
}
