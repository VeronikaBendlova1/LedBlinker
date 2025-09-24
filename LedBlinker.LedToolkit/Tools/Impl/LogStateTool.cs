using LedBlinker.LedToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LedBlinker.LedToolkit.Tools.Impl
{
    public class LogStateTool : ILogStateTool
    {
        
        private HttpClient _client;

        public LogStateTool(HttpClient client)
        {
            _client = client;
        }

        public async Task <List<Logs>> LoadLogsAsync(DateTime? from, DateTime? to)
        {
            

            var url = $"/api/led/logs?from={from:O}&to={to:O}"; // O = round-trip ISO 8601 formát
            var resultResponse = await _client.GetAsync(url); // pošli požadavek na metodu get "/api/led/logs"

            resultResponse.EnsureSuccessStatusCode(); //odpověd je 200OK? je, pokračuje se dál - není vyhodí HttpRequestException

            var resultMessage = await resultResponse.Content.ReadAsStringAsync(); // výsledek response (požadavku) - načti tělo odpovědi pomocí metody ReadAsStringAsync() jako string
            return JsonSerializer.Deserialize<List<Logs>>(resultMessage, new JsonSerializerOptions() // 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
            }) ?? throw new Exception("Deserialization failed");


        }


    }
}
