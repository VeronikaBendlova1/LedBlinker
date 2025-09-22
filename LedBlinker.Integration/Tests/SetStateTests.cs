using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LedBlinker.Integration.Utils;
using LedBlinker.LedToolkit.Tools.Impl;
using LedBlinker.LedToolkit.Tools;
using LedBlinker.Data;
using Microsoft.Extensions.DependencyInjection;
using Humanizer;
using Mono.TextTemplating;
using LedBlinker.LedToolkit.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;

namespace LedBlinker.Integration.Tests
{
    public class SetStateTests
    {
        private LedBlinkerFactory _factory;
        private HttpClient _client;
        private IStateTool _stateTool;


        [SetUp]
        public void Setup()
        {
            _factory = new LedBlinkerFactory();
            _client = _factory.CreateClient();
            _stateTool = new LedStateTool(_client);
        }

        [TearDown]
        public void Clean()
        {
            _factory.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task SetStateDtoReturnsUpdatedLedState()
        {
            //  Připravíme testovací LED v databázi
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Používáme typ z hlavního projektu (LedBlinker.Models)
                db.Leds.Add(new LedBlinker.Models.Led
                {
                    Id = 1,
                    State = (Models.LedState)LedState.Off
                });

                db.SaveChanges();
            }

            //  Vytvoříme DTO s novým stavem
            var dto = new LedStateDto
            {
                State = LedState.Blinking
            };

            //  Zavoláme metodu, která vrací LED
            var result = await _stateTool.SetStateAsync(dto); // metoda vrací vrací LedToolkit.Models.Led

            //  Kontrola – stav LED se změnil na Blinking
            Assert.That(result.State, Is.EqualTo(LedState.Blinking));
        }
        [Test]
        public async Task SetStateAsync_WhenNoLedExists_ReturnsError()
        {
            var dto = new LedStateDto { State = LedState.On };

            var response = await _client.PostAsJsonAsync("/api/led/state", dto);

            

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task SetStateAsync_WhenLedStateInputIsInvalid_ReturnsError()
        {
            /*if (!Enum.IsDefined(typeof(LedState), dto.State))
            result.WithError("Zadej on, off nebo blinking");*/

            var dto = new LedStateDto { State = (LedState) 999 }; // chybně nastavím dto

            var response = await _client.PostAsJsonAsync("/api/led/state", dto); // pošlu chybné dto na controller

            //kontrola těla odpovědi
            var content = await response.Content.ReadAsStringAsync(); // načtu tělo odpovědi

            Assert.That(content, Does.Contain("Zadej on, off nebo blinking"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task SetStateAsync_WhenLedStateInputAndLedAreBothInvalid_ReturnsError()
        {
            /*if (!Enum.IsDefined(typeof(LedState), dto.State))
            result.WithError("Zadej on, off nebo blinking");

            var currentState = await _ledRepo.LoadAny();
            if (currentState == null)*/

            var dto = new LedStateDto { State = (LedState)999 }; // chybně nastavím dto

            // Nic nepřidávám do DB -> žádná LED nebude

            var response = await _client.PostAsJsonAsync("/api/led/state", dto); // pošlu chybné dto na controller

            //kontrola těla odpovědi
            var content = await response.Content.ReadAsStringAsync(); // načtu tělo odpovědi

            // Kontrola, že obsahuje obě hlášky
            Assert.That(content, Does.Contain("Žádná LED není vytvořená"));
            Assert.That(content, Does.Contain("Zadej on, off nebo blinking"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task SetStateAsync_ResponseHasCorrectContentType()
        {
            // DTO s neplatným stavem → očekávám BadRequest
            var dto = new LedStateDto { State = (LedState)999 };

            var response = await _client.PostAsJsonAsync("/api/led/state", dto);

            // Kontrola statusu
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            // Kontrola Content-Type
            Assert.That(response.Content.Headers.ContentType!.MediaType, Is.EqualTo("application/json"));
            Assert.That(response.Content.Headers.ContentType!.CharSet, Is.EqualTo("utf-8"));
        }

    }
}
