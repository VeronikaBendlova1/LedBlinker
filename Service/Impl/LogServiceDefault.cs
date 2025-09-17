using LedBlinker.Models;
using LedBlinker.Repository;

namespace LedBlinker.Service.Impl;

public class LogServiceDefault : ILogService
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
}
