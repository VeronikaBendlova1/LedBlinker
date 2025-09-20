using LedBlinker.Models;

namespace LedBlinker.Repository;
public interface ILogRepo
{
    public Task<Logs> AddLogAsync(Logs log);
    public Task<List<Logs>> GetAllLogsAsync();

    Task<List<Logs>> GetLogsInDateSpanAsync(DateTime from, DateTime to);

}
