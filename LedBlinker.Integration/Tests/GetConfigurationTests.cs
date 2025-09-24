using LedBlinker.Data;
using LedBlinker.Integration.Utils;
using LedBlinker.LedToolkit.Tools.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


namespace LedBlinker.Integration.Tests
{
    internal class GetConfigurationTests
    {


        private LedBlinkerFactory _factory;
        private HttpClient _client;
        private IConfigurationStateTool _logTool;


        [SetUp]
        public void Setup()
        {
            _factory = new LedBlinkerFactory();
            _client = _factory.CreateClient();
            _logTool = new ConfigurationStateTool(_client);
        }

        [TearDown]
        public void Clean()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // kompletně vymaže databázi i identity counter
            db.Database.EnsureDeleted();

            // znovu ji vytvoří (tabulky, schéma atd.)
            db.Database.EnsureCreated();

            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task GetLogs_ReturnsBlinkRate()
        {
            //arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Configurations.Add(new Models.Configuration
                
                        { 
                            BlinkRate = 9, 
                            ConfigurationLed = new() 

                        });
            db.SaveChanges();


            //act
            var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/configuration"));
            resultResponse.EnsureSuccessStatusCode();

            var resultMessage = await resultResponse.Content.ReadAsStringAsync();

            var blinkRateObject = JsonSerializer.Deserialize<float?>(resultMessage, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,


            }) ?? throw new Exception("Deserialization failed");

            var dbRecord = db.Configurations.FirstOrDefault();

            //assert
            Assert.That(dbRecord, Is.Not.Null);
            Assert.That(db.Configurations.Count(), Is.EqualTo(1));
            Assert.That(dbRecord.BlinkRate, Is.EqualTo(9)); //test DB - ověřuju přímo obsah databáze po uložení (tedy že SaveChanges() proběhlo a v DB je opravdu BlinkRate = 9)
            Assert.That(blinkRateObject, Is.EqualTo(9)); //test API - je hodnota, kterou mi vrátilo API po serializaci a deserializaci.

        }
    }
}
