using LedBlinker.Models;

namespace LedBlinker.Service.Impl;

public class LogServiceCustom : ILogService
{
    public Task AddLogAsync(Logs logs)
    {
        Console.WriteLine("Logging from custom service: " + logs.State);
        return Task.CompletedTask;
    }
}
