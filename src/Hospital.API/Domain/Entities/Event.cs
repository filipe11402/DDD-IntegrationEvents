using Hospital.API.Domain.Abstract;

namespace Hospital.API.Domain.Entities;

public class Event : Entity
{
    public string EventName { get; private set; }

    public DateTime DateOccurred { get; private set; }

    public DateTime? ProcessedDate { get; private set; }

    public string Data { get; private set; }

    private Event() { }

    public Event(Guid id, string eventName, DateTime dateOccurred, string data) : base(id)
    {
        EventName = eventName;
        DateOccurred = dateOccurred;
        Data = data;
    }

    public void Processed() 
    {
        ProcessedDate = DateTime.UtcNow;
    }
}
