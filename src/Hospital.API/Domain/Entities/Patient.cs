using Hospital.API.Domain.Abstract;

namespace Sales.API.Domain.Entities;

public class Patient
{
    public Guid Id { get; init; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Address { get; private set; }

    public List<IDomainEvent> DomainEvents { get; private set; }

    private Patient() { }

    public Patient(Guid id, string name, string email, string address)
    {
        Id = id;
        Name = name;
        Email = email;
        Address = address;
    }

    public void AddDomainEvent(IDomainEvent domainEvent) 
    {
        if (DomainEvents is null) 
        {
            DomainEvents = new List<IDomainEvent>();
        }

        DomainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() 
    {
        DomainEvents?.Clear();
    }
}
