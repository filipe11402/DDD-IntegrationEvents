namespace Sales.API.Infrastructure.Events;

public class PatientCreatedIntegrationEvent : IIntegrationEvent
{
    public string Type { get; private set; }

    public string Data { get; private set; }

    public PatientCreatedIntegrationEvent(string data)
    {
        Type = "PatientCreated";
        Data = data;
    }
}
