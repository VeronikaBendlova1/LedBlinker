using FluentResults;
using LedBlinker.Models;

namespace LedBlinker.Repository;

public interface ILedRepo
{
    public Task<Led> LoadAny();
    Task<Led> SetupNew();
    Task<Result<Led>> UpdateAsync(Led currentState);
}
