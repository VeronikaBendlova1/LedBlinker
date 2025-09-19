using FluentResults;
using LedBlinker.Models;

namespace LedBlinker.Service.Impl
{
    public interface IConfigurationServiceDefault
    {
        public Task<float?> GetBlinkRateAsync();

        public Task<Result<float>> PostBlinkRateAsync(ConfigurationDto dto);

    }
}
