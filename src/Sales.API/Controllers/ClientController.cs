using Microsoft.AspNetCore.Mvc;
using Sales.API.Domain.Entities;
using Sales.API.Domain.Repositories;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetClient([FromQuery] Guid id) 
        {
            Client client = await _clientRepository.GetById(id);

            return client is null ?
                NotFound() :
                Ok(client);
        }

        //[HttpPost]
        //[Route("create")]
        //public async Task<IActionResult> Create([FromBody] CreateClientRequest request) 
        //{
        //    var client = new Client(
        //        Guid.NewGuid(),
        //        request.Name,
        //        request.Email,
        //        request.Address
        //        );

        //    client = await _clientRepository.Add(client);

        //    return StatusCode(
        //        StatusCodes.Status201Created,
        //        new { Id = client.Id }
        //        );
        //}
    }
}
