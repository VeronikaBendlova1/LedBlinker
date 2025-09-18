using FluentResults;
using LedBlinker.Models;
using LedBlinker.Repository;

namespace LedBlinker.Service.Impl;

public class LedStateService : ILedStateService
{
    private readonly ILedRepo _ledRepo;
    private readonly ILogServiceDefault _logService;

    public LedStateService(ILogServiceDefault logService)
    {
        _logService = logService;
    }

    public LedStateService(ILedRepo ledRepo)
    {
        _ledRepo = ledRepo;
    }

    public async Task<Led> LoadAsync()
    {
        var currentState = await _ledRepo.LoadAny();

        if (currentState == null)
            currentState = await _ledRepo.SetupNew();

        return currentState;
    }

    public async Task<Result<Led>> SetStateAsync(LedStateDto dto)
    {
        var result = new Result<Led>();

        var currentState = await _ledRepo.LoadAny();
        if (currentState == null)
            result.WithError("Žádná LED není vytvořená");
        if (!Enum.IsDefined(typeof(LedState), dto.State))
            result.WithError("Zadej on, off nebo blinking");

        if (result.IsFailed)
            return result;
        currentState!.State = dto.State;

        var updatedLed = await _ledRepo.UpdateAsync(currentState);

        await _logService.AddLogAsync(new Logs
        {
            Date = DateTime.Now,
            State = dto.State.ToString()
        });
        return updatedLed;
    }
}
