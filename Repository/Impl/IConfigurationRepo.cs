using FluentResults;
using LedBlinker.Models;

namespace LedBlinker.Repository.Impl
{
    public interface IConfigurationRepo
    {

        public Task<Configuration?> LoadConfiguration();

        public Task<Configuration?> AddNewConfigurationAsync(ConfigurationDto dto, Led led);

        public Task<Result<Configuration>> UpdateAsync(Configuration currentState);

        
    }
}
