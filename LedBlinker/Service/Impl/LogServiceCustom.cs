using LedBlinker.Models;
using LedBlinker.Repository;

namespace LedBlinker.Service.Impl;

public class LogServiceCustom : ILogServiceDefault
{
    private readonly ILogRepo _logRepo;

    public LogServiceCustom(ILogRepo logRepo)
    {
        _logRepo = logRepo;
    }


    public Task AddLogAsync(Logs logs)
    {
        Console.WriteLine("Logging from custom service: " + logs.State);
        return Task.CompletedTask;
    }

    public async Task<List<Logs>> GetLogsAsync(DateTime? from, DateTime? to)
    {
        // Awaitneme Task, abychom získali List<Logs> - Teï je 'logs' opravdu List, na který lze použít Where()
        var logs = await _logRepo.GetAllLogsAsync(); //bez await nezískáme list, na který jde aplikovat Where(),protože Task je jen slib, že v budoucnu získáte výsledek (seznam logù)

        if (from.HasValue)
            logs = logs.Where(l => l.Date >= from.Value).ToList();//vytvoøí nový seznam
        if (to.HasValue)
            logs = logs.Where(l => l.Date <= to.Value).ToList(); //// .ToList() je nutný pro vytvoøení nového, filtrovaného seznamu

        //vrací hotový seznam
        return logs;
    }

    public Task<List<Logs>> LoadLogsInDateSpanAsync(DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }
}
