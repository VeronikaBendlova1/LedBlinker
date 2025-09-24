using LedBlinker.Data;
using LedBlinker.Models;
using LedBlinker.Service;
using LedBlinker.Service.Impl;
using Microsoft.AspNetCore.Mvc;

namespace LedBlinker.Controllers
{
    [ApiController] ////https://github.com/VeronikaBendlova1/LedBlinker/blob/master/Controllers/LedController.cs
    [Route("api/led")]
    public class LedController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILedStateService _ledStateService;
        private readonly ILogServiceDefault _logService;
        private readonly IConfigurationServiceDefault _configuration;



        public LedController(
            ILedStateService ledStateService,
            ApplicationDbContext db,
            IConfigurationServiceDefault configuration,
            ILogServiceDefault logService)
        {
            _ledStateService = ledStateService;
            _db = db;
            _configuration = configuration;
            _logService = logService;
        }


        [HttpGet("state")]
        public async Task<IActionResult> GetState()
        {
            var stavLedky = await _ledStateService.LoadAsync();
            return Ok(stavLedky);
        }

        [HttpPost("state")]
        public async Task<IActionResult> SetState([FromBody] LedStateDto dto)
        {

            var led = await _ledStateService.SetStateAsync(dto);
            return Ok(led.Value); //vracím skutečnou ledku bez obalu Result<Led>
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogsAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var logs = await _logService.GetLogsAsync(from, to);
            return Ok(logs);
        }

        // new version 
        [HttpGet("logswithbetterfilter")]
        public async Task<IActionResult> GetLogsAsyncWithBetterFilter([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (from == null || to == null)
                return BadRequest("Musíte zadat oba parametry: from i to.");

            var logs = await _logService.LoadLogsInDateSpanAsync(from.Value, to.Value);
            return Ok(logs);
        }

        [HttpPost("configuration")]
        public async Task<IActionResult> PostConfiguration([FromBody] ConfigurationDto dto)
        {
            var blinkRate = await _configuration.PostBlinkRateAsync(dto);

            return Ok(blinkRate.Value);
        }

        [HttpGet("configuration")]
        public async Task<IActionResult> GetConfiguration()
        {
            var config = await _configuration.GetBlinkRateAsync();
            return Ok(config);
        }
    }
}
