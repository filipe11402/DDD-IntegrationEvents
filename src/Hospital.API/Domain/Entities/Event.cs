namespace Hospital.API.Domain.Entities;

public class Event
{
    public Guid Id { get; init; }

    public string EventName { get; private set; }

    public DateTime DateOccurred { get; private set; }

    public string Data { get; private set; }

    public Event(Guid id, string eventName, DateTime dateOccurred, string data)
    {
        Id = id;
        EventName = eventName;
        DateOccurred = dateOccurred;
        Data = data;
    }
}
