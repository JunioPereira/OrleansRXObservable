using GrainInterfaces.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using States.Models;

namespace OrleansRXObservable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        ILogger<CustomerController> _logger { get; }
        IClusterClient iClusterClient { get; }

        public CustomerController(ILogger<CustomerController> logger, IClusterClient _iClusterClient)
        {
            _logger = logger;
            iClusterClient = _iClusterClient;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var igrain = iClusterClient.GetGrain<ICustomerGrain>(id);

            var result = await igrain.Get();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CustomerState state)
        {
            var igrain = iClusterClient.GetGrain<ICustomerGrain>(state.Id);

            await igrain.Set(state);

            return Ok();
        }
    }
}
