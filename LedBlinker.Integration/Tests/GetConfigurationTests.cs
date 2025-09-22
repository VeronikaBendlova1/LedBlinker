using LedBlinker.Data;
using LedBlinker.Integration.Utils;
using LedBlinker.LedToolkit.Tools.Impl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LedBlinker.LedToolkit.Models;
using System.Net;


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
            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task GetLogs_ReturnsBlinkRate()
        {
            //arrange
            var scope = _factory.Services.CreateScope() ;
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            
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
            Assert.That(dbRecord.BlinkRate, Is.EqualTo(9));
        }
    }
}
