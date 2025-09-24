using FluentResults;
using LedBlinker.Data;
using LedBlinker.Models;
using Microsoft.EntityFrameworkCore;

namespace LedBlinker.Repository.Impl
{
    public class ConfigurationRepo : IConfigurationRepo
    {

        private readonly ApplicationDbContext _db;
      
        public ConfigurationRepo(ApplicationDbContext db)
        {
            _db = db;
            
        }

        
        public async Task<Result<Configuration>> UpdateAsync(Configuration currentState)
        {
            var updated = _db.Configurations.Update(currentState);
            await _db.SaveChangesAsync();
            return Result.Ok(updated.Entity);
        }

        public Task<Configuration?> LoadConfiguration()
        {
            return _db.Configurations.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Configuration?> AddNewConfigurationAsync(ConfigurationDto dto, Led led)
        {
            _db.Leds.Attach(led);
            await _db.SaveChangesAsync();
            var newConfig = new Configuration()
            {
                BlinkRate = dto.BlinkRate,
                ConfigurationLed = led
            };
            var result = _db.Configurations.Add(newConfig);
            await _db.SaveChangesAsync();

            return result.Entity;
        }

        
    }
}
