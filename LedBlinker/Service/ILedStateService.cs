
using FluentResults;
using LedBlinker.Models;

namespace LedBlinker.Service;

public interface ILedStateService
{
    Task<Led> LoadAsync();
    Task<Result<Led>> SetStateAsync(LedStateDto dto);
}
