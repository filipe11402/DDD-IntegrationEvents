using Hospital.API.Mediator.Commands.CreatePatient;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;
using Sales.API.DTOs;

namespace Hospital.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        private readonly IMediator _mediator;

        public PatientController(
            IPatientRepository patientRepository,
            IMediator mediator)
        {
            _patientRepository = patientRepository;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
        {
            var patient = new Patient(
                Guid.NewGuid(),
                request.Name,
                request.Email,
                request.Address
                );

            patient = await _mediator.Send(
                new CreatePatientCommand(patient)
                );

            return StatusCode(
                StatusCodes.Status201Created,
                new { Id = patient.Id }
                );
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetPatient([FromQuery] Guid id)
        {
            Patient patient = await _patientRepository.GetById(id);

            return patient is null ?
                NotFound() :
                Ok(patient);
        }
    }
}
