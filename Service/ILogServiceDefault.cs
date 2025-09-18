using LedBlinker.Models;

namespace LedBlinker.Service;

public interface ILogServiceDefault
{
    Task AddLogAsync(Logs logs);

    // Task AddLogAsync(Logs logs);

    Task<List<Logs>> GetLogsAsync(DateTime? from, DateTime? to); //Service vrstva te� dostane logy z repozit��e a aplikuje filtry
}
