using Hospital.API.Domain.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sales.API.Domain.Entities;

public class Patient : AggregateRoot
{
    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Address { get; private set; }

    private Patient() { }

    public Patient(Guid id, string name, string email, string address) : base(id)
    {
        Name = name;
        Email = email;
        Address = address;
    }
}
