using LedBlinker.Data;
using LedBlinker.Models;
using LedBlinker.Service;
using Microsoft.AspNetCore.Mvc;

namespace LedBlinker.Controllers
{
    [ApiController]
    [Route("api/led")]
    public class LedController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILedStateService _ledStateService;

        public LedController(ApplicationDbContext db) => _db = db;

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
        public IActionResult GetLogs([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var logs = _db.Logs.AsQueryable();

            if (from.HasValue)
                logs = logs.Where(l => l.Date >= from.Value);
            if (to.HasValue)
                logs = logs.Where(l => l.Date <= to.Value);

            return Ok(logs.ToList());
        }

        [HttpPost("configuration")]
        public IActionResult PostConfiguration([FromBody] ConfigurationDto dto)
        {
            var led = _db.Leds.FirstOrDefault();
            if (led == null)
                return NotFound("Žádná LED není vytvořená");

            if (led.State != LedState.Blinking)
                return BadRequest("Ledka nebliká, je nutné ji nejdřív přepnout na blikání");

            if (dto.BlinkRate <= 0 || dto.BlinkRate > 10)
                return BadRequest("BlinkRate musí být větší než 0 a menší než 10");

            var config = _db.Configurations.FirstOrDefault();

            if (config == null)
            {
                config = new Configuration
                {
                    BlinkRate = dto.BlinkRate,
                    ConfigurationLed = led
                };
                _db.Configurations.Add(config);
            }
            else
            {
                config.BlinkRate = dto.BlinkRate;
            }

            _db.SaveChanges();
            return Ok(config.BlinkRate);
        }

        [HttpGet("configuration")]
        public IActionResult GetConfiguration()
        {
            var config = _db.Configurations.Select(x => new { x.BlinkRate }).FirstOrDefault();
            return Ok(config);
        }
    }
}
