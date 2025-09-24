using LedBlinker.LedToolkit.Models;

namespace LedBlinker.LedToolkit.Tools.Impl
{
    public  interface IConfigurationStateTool
    {
        public Task<float?> LoadConfigurationState();
        public Task<float?> SetConfigurationStateAsync(ConfigurationDto dto);
    }
}
