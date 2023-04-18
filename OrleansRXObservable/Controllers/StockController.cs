using GrainInterfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;
using States.Models;

namespace OrleansRXObservable.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        IClusterClient iClusterClient { get; }

        public StockController(ILogger<StockController> logger, IClusterClient _iClusterClient)
        {
            _logger = logger;
            iClusterClient = _iClusterClient;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> Get(string symbol)
        {
            var igrain = iClusterClient.GetGrain<IStockGrain>(symbol);

            var result = await igrain.Get();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]StockState stock)
        {
            var igrain = iClusterClient.GetGrain<IStockGrain>(stock.Symbol);

            await igrain.Set(stock);

            return Ok();
        }
    }
}