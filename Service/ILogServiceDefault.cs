using LedBlinker.Models;

namespace LedBlinker.Service;

public interface ILogServiceDefault
{
    Task AddLogAsync(Logs logs);

    // Task AddLogAsync(Logs logs);

    Task<List<Logs>> GetLogsAsync(DateTime? from, DateTime? to); //Service vrstva teï dostane logy z repozitáøe a aplikuje filtry
}
