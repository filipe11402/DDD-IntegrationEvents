namespace Sales.API.Domain.Entities;

public class Client
{
    //[]
    public Guid Id { get; init; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Address { get; private set; }

    private Client() { }

    public Client(Guid id, string name, string email, string address)
    {
        Id = id;
        Name = name;
        Email = email;
        Address = address;
    }
}
