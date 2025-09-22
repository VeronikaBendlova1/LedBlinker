using LedBlinker.LedToolkit.Models;

namespace LedBlinker.LedToolkit.Tools;

public interface IStateTool
{

    public Task<Led> LoadState();

    public Task<Led> SetStateAsync(LedStateDto dto);
}
