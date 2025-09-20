
using FluentResults;
using LedBlinker.Models;
using LedBlinker.Repository;
using LedBlinker.Repository.Impl;

namespace LedBlinker.Service.Impl
{
    public class ConfigurationServiceDefault : IConfigurationServiceDefault
    {
        private readonly ILedRepo _ledRepo;
        private readonly IConfigurationRepo _configurationRepo;



        public ConfigurationServiceDefault(
            IConfigurationRepo configRepo,
            ILedRepo ledRepo)

        {
            _ledRepo = ledRepo;
            _configurationRepo = configRepo;
        }

        public async Task<float?> GetBlinkRateAsync()
        {
            var configuration = await _configurationRepo.LoadConfiguration();

            return configuration?.BlinkRate;
        }


        public async Task<Result<float>> PostBlinkRateAsync(ConfigurationDto dto)
        {
            var result = new Result<float>();

            // 1) Načtu LED
            var led = await _ledRepo.LoadAny();
            if (led == null)
                return result.WithError("Žádná LED není vytvořená");

            // 2) Zkontroluju stav LED
            if (led.State != LedState.Blinking)
                result.WithError("Ledka nebliká, je nutné ji nejdřív přepnout na blikání");

            // 3) Zkontroluju vstup
            if (dto.BlinkRate <= 0 || dto.BlinkRate > 10)
                result.WithError("BlinkRate musí být větší než 0 a menší než 10");

            if (result.IsFailed) return result;

            // 4) Uložím config
            var config = await _configurationRepo.LoadConfiguration();

            if (config == null)
            {
                config = await _configurationRepo.AddNewConfigurationAsync(dto, led);
            }
            else
            {
                config.BlinkRate = dto.BlinkRate;
                await _configurationRepo.UpdateAsync(config); // nezapomenout změnu uložit
            }

            if (config == null) return Result.Fail("Konfigurace Neexistuje");

            return Result.Ok(config.BlinkRate);
        }
    }
}
