using Hospital.API.Domain.Abstract;
using Hospital.API.Domain.Events;
using Hospital.API.Infrastructure.Mappers;
using Hospital.API.Tests.Fakes;

namespace Hospital.API.Tests.Infrastructure;

public class EventMapperTests
{
    private EventMapper _eventMapper;

    public EventMapperTests()
    {
        _eventMapper = new();
    }

    [Fact]
    public void MapDomainEvent_PatientCreatedEvent_ReturnsMappedIntegrationEvent() 
    {
        //Arrange
        var domainEvent = new PatientCreatedDomainEvent(
            Guid.NewGuid(),
            "patientname",
            "patient@email.com",
            "patient address portugal"
            );

        //Act
        IIntegrationEvent? act = _eventMapper.MapDomainEvent(domainEvent);

        //Assert
        act.Should()
           .BeOfType<PatientCreatedIntegrationEvent>();
        act.As<PatientCreatedIntegrationEvent>()
           .Id.Should().Be(domainEvent.Id);
        act.As<PatientCreatedIntegrationEvent>()
           .Name.Should().Be(domainEvent.Name);
        act.As<PatientCreatedIntegrationEvent>()
           .Email.Should().Be(domainEvent.Email);
        act.As<PatientCreatedIntegrationEvent>()
           .Address.Should().Be(domainEvent.Address);
    }

    [Fact]
    public void MapDomainEvent_UknownEvent_ReturnsNull()
    {
        //Arrange
        var domainEvent = new FakeDomainEvent();

        //Act
        IIntegrationEvent? act = _eventMapper.MapDomainEvent(domainEvent);

        //Assert
        act.Should()
           .BeNull();
    }
}
