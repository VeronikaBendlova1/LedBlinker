using LedBlinker.Data;
using LedBlinker.Models;
using LedBlinker.Repository.Impl;
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
        public IActionResult SetState([FromBody] LedStateDto dto)
        {

            var led = _ledStateService.SetStateAsync(dto);
            return Ok(led);
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogsAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var logs = await _logService.GetLogsAsync(from, to);
            return Ok(logs);
        }

        // new version 
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogsAsyncWithBetterFilter([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (from == null || to == null)
                return BadRequest("Musíte zadat oba parametry: from i to.");

            var logs = await _logService.LoadLogsInDateSpanAsync(from.Value, to.Value);
            return Ok(logs);
        }

        [HttpPost("configuration")]
        public IActionResult PostConfiguration([FromBody] ConfigurationDto dto)
        {
            var blinkRate = _configuration.PostBlinkRateAsync(dto);

            return Ok(blinkRate);
        }

        [HttpGet("configuration")]
        public async Task<IActionResult> GetConfiguration()
        {
            var config = await _configuration.GetBlinkRateAsync();
            return Ok(config);
        }
    }
}
