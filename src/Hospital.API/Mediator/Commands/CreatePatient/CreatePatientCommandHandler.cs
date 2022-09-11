using MediatR;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;

namespace Hospital.API.Mediator.Commands.CreatePatient;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Patient>
{
    private readonly IPatientRepository _patientRepository;

    public CreatePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Patient> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        return await _patientRepository.Add(request.Patient);
    }
}
