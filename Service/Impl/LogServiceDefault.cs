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
        // Awaitneme Task, abychom z�skali List<Logs> - Te� je 'logs' opravdu List, na kter� lze pou��t Where()
        var logs = await _logRepo.GetAllLogsAsync(); //bez await nez�sk�me list, na kter� jde aplikovat Where(),proto�e Task je jen slib, �e v budoucnu z�sk�te v�sledek (seznam log�)

        if (from.HasValue)
            logs = logs.Where(l => l.Date >= from.Value).ToList();//vytvo�� nov� seznam
        if (to.HasValue)
            logs = logs.Where(l => l.Date <= to.Value).ToList(); //// .ToList() je nutn� pro vytvo�en� nov�ho, filtrovan�ho seznamu

        //vrac� hotov� seznam
        return logs;
    }
    public async Task<List<Logs>> LoadLogsInDateSpanAsync(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("Zadejte pros�m platn� rozmen� od - do");

        return await _logRepo.GetLogsInDateSpanAsync(from, to);
    }
}
