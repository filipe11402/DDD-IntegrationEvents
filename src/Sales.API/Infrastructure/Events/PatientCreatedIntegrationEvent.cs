using Sales.API.Domain.Events;

namespace Sales.API.Infrastructure.Events;

public record PatientCreatedIntegrationEvent(Guid Id, string Name, string Email, string Address) : IIntegrationEvent;
