using Hospital.API.Domain.Events;
using Hospital.API.Domain.Repositories;
using MediatR;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;

namespace Hospital.API.Mediator.Commands.CreatePatient;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Patient>
{
    private readonly IPatientRepository _patientRepository;

    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Patient> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        Patient patient = await _patientRepository.Add(request.Patient);

        patient.AddDomainEvent(
            new PatientCreatedDomainEvent(patient.Id, patient.Name, patient.Email, patient.Address)
            );

        await _unitOfWork.Commit(cancellationToken);

        return patient;
    }
}
