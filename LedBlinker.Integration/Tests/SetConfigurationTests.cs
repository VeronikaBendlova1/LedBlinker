using LedBlinker.Data;
using LedBlinker.Integration.Utils;
using LedBlinker.LedToolkit.Models;
using LedBlinker.LedToolkit.Tools.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace LedBlinker.Integration.Tests
{
    internal class SetConfigurationTests
    {


        private LedBlinkerFactory _factory;
        private HttpClient _client;
        private IConfigurationStateTool _configTool;


        [SetUp]
        public void Setup()
        {

            _factory = new LedBlinkerFactory();
            _client = _factory.CreateClient();
            _configTool = new ConfigurationStateTool(_client);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

        }

        [TearDown]
        public void Clean()
        {

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task SetConfiguration_ReturnsUpdatedBlinkRate()
        {
            //arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Používáme typ z hlavního projektu (LedBlinker.Models)
                var led = db.Leds.Add(new LedBlinker.Models.Led
                {
                    State = LedBlinker.Models.LedState.Blinking
                });
                
                db.SaveChanges();
            }
            //  Vytvoříme DTO s novým stavem
            var dto = new ConfigurationDto
            {
                BlinkRate = 2
            };

            //  act Zavoláme metodu, která vrací float
            var result = await _configTool.SetConfigurationStateAsync(dto); 

            //assert
            Assert.That(result, Is.EqualTo(2));
            Assert.That(result.GetType(), Is.EqualTo(typeof(float)));
            




        }
    }
}
