using System.Text.Json;
using LedBlinker.Data;
using LedBlinker.Integration.Utils;
using LedBlinker.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LedBlinker.Integration.Tests;


public class GetStateTests
{
    private LedBlinkerFactory _factory;
    private HttpClient _client;

    //Runs before each test
    [SetUp]
    public void Setup()
    {
        _factory = new LedBlinkerFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void Clean()
    {
        _factory.Dispose();
        _client.Dispose();
    }

    [Test]
    public async Task LoadLedState_WhenDatabaseIsEmpty_Always()
    {
        var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/state"));
        Assert.That(resultResponse.IsSuccessStatusCode);
        var resultMessage = await resultResponse.Content.ReadAsStringAsync();
        var resultObject = JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        });

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbRecord = db.Leds.FirstOrDefault();
        Assert.That(dbRecord, Is.Not.Null);
        Assert.That(dbRecord.Id, Is.EqualTo(resultObject.Id));
    }

    [Test]
    public async Task LoadLedState_Always()
    {
        //create Led record in database
        Led dbLed;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var req = db.Leds.Add(new()
            {
                Id = 5,
                State = LedState.On
            });
            db.SaveChanges();
            dbLed = req.Entity;
        }

        var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/state"));
        Assert.That(resultResponse.IsSuccessStatusCode);
        var resultMessage = await resultResponse.Content.ReadAsStringAsync();
        var resultObject = JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        });
        //Validate that exising record is returned instead of creating new one
        Assert.That(dbLed.Id, Is.EqualTo(resultObject.Id));
    }

}
