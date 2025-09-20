using LedBlinker.Models;
using LedBlinker.Repository;

namespace LedBlinker.Service.Impl;

public class LogServiceDefault : ILogServiceDefault
{
    private readonly ILogRepo _logRepo;

    public LogServiceDefault(ILogRepo logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task AddLogAsync(Logs logs)
    {
        await _logRepo.AddLogAsync(logs);
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
    public async Task<List<Logs>> LoadLogsInDateSpanAsync(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("Zadejte prosím platné rozmení od - do");

        return await _logRepo.GetLogsInDateSpanAsync(from, to);
    }
}
