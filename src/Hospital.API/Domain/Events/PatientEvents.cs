using Hospital.API.Domain.Abstract;

namespace Hospital.API.Domain.Events;

public record PatientCreatedDomainEvent(Guid Id, string Name, string Email, string Address) : IDomainEvent;

public record PatientCreatedIntegrationEvent(Guid Id, string Name, string Email, string Address) : IIntegrationEvent;
