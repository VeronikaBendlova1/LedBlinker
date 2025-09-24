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
    [SetUp] // Framework(NUnit) to vol� automaticky p�ed a po ka�d�m testu
    public void Setup()
    {
        _factory = new LedBlinkerFactory();
        _client = _factory.CreateClient();
        _stateTool = new LedStateTool(_client);
    }

    [TearDown] // Framework(NUnit) to vol� automaticky p�ed a po ka�d�m testu
    public void Clean()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        /*db.Leds.RemoveRange(db.Leds);
        db.Configurations.RemoveRange(db.Configurations);
        db.SaveChanges(); toto sma�e jen tabulky, ale nezresetuje cel� InMemory context*/

        // kompletn� vyma�e datab�zi i identity counter - ��ta� ID Ka�d� test te� za�ne s pr�zdnou datab�z�, Id od 1.
        db.Database.EnsureDeleted();

        // znovu ji vytvo�� (tabulky, sch�ma atd.)
        db.Database.EnsureCreated();

        _factory.Dispose();
        _client.Dispose();
    }
    
    [Test]
    public async Task LoadLedState_WhenDatabaseIsEmpty_Always() //DB je pr�zdn� ? API vytvo�� nov� z�znam�
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
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //scope � izolovan� kontejner pro lifetime scoped slu�by. //Ze scope m��e� vyt�hnout slu�by
        var dbRecord = db.Leds.FirstOrDefault();
        Assert.That(dbRecord, Is.Not.Null);
        Assert.That(dbRecord.Id, Is.EqualTo(resultObject.Id));
        
    }

    [Test]
    public async Task LoadLedState_Always() //ov��uje sc�n�� �DB u� n�co m� ? API vr�t� existuj�c� z�znam�.
    {
        //create Led record in database
        Led dbLed;
        using (var scope = _factory.Services.CreateScope()) //Takto se v testech dostanu do datab�ze a m��u ji kontrolovat.
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //Takto se v testech dostanu do datab�ze a m��u ji kontrolovat.
            var req = db.Leds.Add(new()
            {
                // Id = 5, -> ? Pak se ID vytvo�� automaticky od 1
                State = LedState.On
            });
            db.SaveChanges();
            dbLed = req.Entity; // req.Entity = reference na pr�v� ulo�en� objekt, po SaveChanges() s vypln�n�mi hodnotami, proto�e db.Leds.Add(...) vrac� EntityEntry<Led>
        }

        var result = await _stateTool.LoadState();  //Use toolkit for request toolkit ud�l� HTTP GET na /api/led/state a vr�t� Led.

        //Validate that exising record is returned instead of creating new one
        Assert.That(dbLed.Id, Is.EqualTo(result.Id)); // ov��en�, �e api nevr�tilo n�co nov�ho ID z datab�ze = ID z api

    }
    [Test]
    public async Task LoadLedState_WhenLedIsBlinking_ReturnBlinking() //validace hodnot(State mus�  Blinking),
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

        var result = await _stateTool.LoadState(); // vrac� stav co si zalo�ilo API

        Assert.That(dbLed.State, Is.EqualTo(LedState.Blinking));
        Assert.That(result.Id, Is.EqualTo(1));




    }

}
