using System.Text.Json;
using LedBlinker.Data;
using LedBlinker.Integration.Utils;
using LedBlinker.LedToolkit.Tools;
using LedBlinker.LedToolkit.Tools.Impl;
using LedBlinker.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LedBlinker.Integration.Tests;


public class GetStateTests
{
    private LedBlinkerFactory _factory;
    private HttpClient _client;
    private IStateTool _stateTool;

    //Runs before each test
    [SetUp] // Framework(NUnit) to volá automaticky pøed a po každém testu
    public void Setup()
    {
        _factory = new LedBlinkerFactory();
        _client = _factory.CreateClient();
        _stateTool = new LedStateTool(_client);
    }

    [TearDown] // Framework(NUnit) to volá automaticky pøed a po každém testu
    public void Clean()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        /*db.Leds.RemoveRange(db.Leds);
        db.Configurations.RemoveRange(db.Configurations);
        db.SaveChanges(); toto smaže jen tabulky, ale nezresetuje celý InMemory context*/

        // kompletnì vymaže databázi i identity counter - èítaè ID Každý test teï zaène s prázdnou databází, Id od 1.
        db.Database.EnsureDeleted();

        // znovu ji vytvoøí (tabulky, schéma atd.)
        db.Database.EnsureCreated();

        _factory.Dispose();
        _client.Dispose();
    }
    
    [Test]
    public async Task LoadLedState_WhenDatabaseIsEmpty_Always() //DB je prázdná ? API vytvoøí nový záznam“
    {

        

        
       var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/state")); //Use standard http request

        Assert.That(resultResponse.IsSuccessStatusCode);
        var resultMessage = await resultResponse.Content.ReadAsStringAsync();
        var resultObject = JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        });

        var result = await _stateTool.LoadState();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //scope – izolovaný kontejner pro lifetime scoped služby. //Ze scope mùžeš vytáhnout služby
        var dbRecord = db.Leds.FirstOrDefault();
        Assert.That(dbRecord, Is.Not.Null);
        Assert.That(dbRecord.Id, Is.EqualTo(resultObject.Id));
        
    }

    [Test]
    public async Task LoadLedState_Always() //ovìøuje scénáø „DB už nìco má ? API vrátí existující záznam“.
    {
        //create Led record in database
        Led dbLed;
        using (var scope = _factory.Services.CreateScope()) //Takto se v testech dostanu do databáze a mùžu ji kontrolovat.
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //Takto se v testech dostanu do databáze a mùžu ji kontrolovat.
            var req = db.Leds.Add(new()
            {
                // Id = 5, -> ? Pak se ID vytvoøí automaticky od 1
                State = LedState.On
            });
            db.SaveChanges();
            dbLed = req.Entity; // req.Entity = reference na právì uložený objekt, po SaveChanges() s vyplnìnými hodnotami, protože db.Leds.Add(...) vrací EntityEntry<Led>
        }

        var result = await _stateTool.LoadState();  //Use toolkit for request toolkit udìlá HTTP GET na /api/led/state a vrátí Led.

        //Validate that exising record is returned instead of creating new one
        Assert.That(dbLed.Id, Is.EqualTo(result.Id)); // ovìøení, že api nevrátilo nìco nového ID z databáze = ID z api

    }
    [Test]
    public async Task LoadLedState_WhenLedIsBlinking_ReturnBlinking() //validace hodnot(State musí  Blinking),
    {
        Led dbLed;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var req = db.Leds.Add(new Led
            {
                Id = 1,
                State = LedState.Blinking
            }
            );
            db.SaveChanges();
            dbLed = req.Entity;
        }

        var result = await _stateTool.LoadState(); // vrací stav co si založilo API

        Assert.That(dbLed.State, Is.EqualTo(LedState.Blinking));
        Assert.That(result.Id, Is.EqualTo(1));




    }

}
