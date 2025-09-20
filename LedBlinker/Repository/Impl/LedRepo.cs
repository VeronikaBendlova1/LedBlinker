using FluentResults;
using LedBlinker.Data;
using LedBlinker.Models;
using Microsoft.EntityFrameworkCore;

namespace LedBlinker.Repository.Impl;


public class LedRepo : ILedRepo
{
    private readonly ApplicationDbContext _db;

    public LedRepo(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Led?> LoadAny() // na�te led
    {
        return _db.Leds.AsNoTracking().FirstOrDefaultAsync(); // vrac� ledku _db.Leds.FirstOrDefault();
    }

    public async Task<Led> SetupNew() // vytvo�� novou ledku a vr�t� ji
    {

        var defaultConfig = new Led()
        {
            State = LedState.Off
        };
        var result = await _db.Leds.AddAsync(defaultConfig);
        await _db.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Result<Led>> UpdateAsync(Led currentState)
    {
        var updated = _db.Leds.Update(currentState);
        await _db.SaveChangesAsync();
        return Result.Ok(updated.Entity);
    }
}
