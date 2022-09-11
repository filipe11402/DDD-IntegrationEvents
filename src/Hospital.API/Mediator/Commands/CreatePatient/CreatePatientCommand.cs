using MediatR;
using Sales.API.Domain.Entities;

namespace Hospital.API.Mediator.Commands.CreatePatient;

public class CreatePatientCommand : IRequest<Patient>
{
    public Patient Patient { get; }

    public CreatePatientCommand(Patient patient)
    {
        Patient = patient;
    }
}
