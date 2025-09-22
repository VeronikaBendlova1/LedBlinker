using LedBlinker.Data;
using LedBlinker.Integration.Utils;
using LedBlinker.LedToolkit.Tools.Impl;
using LedBlinker.LedToolkit.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LedBlinker.LedToolkit.Models;
using Microsoft.Extensions.DependencyInjection;
using LedBlinker.Models;
using Logs = LedBlinker.Models.Logs;
using System.Text.Json;

namespace LedBlinker.Integration.Tests
{
    public class GetLogsTests
    {

        private LedBlinkerFactory _factory;
        private HttpClient _client;
        private ILogStateTool _logTool;


        [SetUp]
        public void Setup()
        {
            _factory = new LedBlinkerFactory();
            _client = _factory.CreateClient();
            _logTool = new LogStateTool(_client);
        }

        [TearDown]
        public void Clean()
        {
            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task GetLogs_ReturnsLogsWithinDateRange()
        {
            // Arrange – připravím DB
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Logs.Add(new Logs
                {
                    Id = 1,
                    State = "On",
                    Date = new DateTime(2025, 1, 1)
                });

                db.Logs.Add(new Logs
                {
                    Id = 2,
                    State = "Off",
                    Date = new DateTime(2025, 2, 1)
                });

                db.SaveChanges();
            }

            var from = new DateTime(2025, 1, 30);
            var to = new DateTime(2025, 2, 20);

            // Act – volám přímo API přes HttpClient
            var url = $"/api/led/logs?from={from:O}&to={to:O}";
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var logs = JsonSerializer.Deserialize<List<Logs>>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true
            });

            // Assert – kontroluju výsledek, který API vrací
            Assert.That(logs, Is.Not.Null);
            Assert.That(logs.Count, Is.EqualTo(1));
            Assert.That(logs.First().Date, Is.EqualTo(new DateTime(2025, 2, 1)));
            Assert.That(logs.First().State, Is.EqualTo("Off"));
        }
    }
}
