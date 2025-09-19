using LedBlinker.Models;

namespace LedBlinker.Service;

public interface ILogServiceDefault
{
    Task AddLogAsync(Logs logs);

    // Task AddLogAsync(Logs logs);
    public Task<List<Logs>> LoadLogsInDateSpanAsync(DateTime from, DateTime to);
    Task<List<Logs>> GetLogsAsync(DateTime? from, DateTime? to); //Service vrstva te� dostane logy z repozit��e a aplikuje filtry
}
