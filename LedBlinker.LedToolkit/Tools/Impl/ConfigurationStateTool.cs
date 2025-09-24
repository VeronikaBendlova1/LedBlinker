using LedBlinker.LedToolkit.Models;
using System.Text.Json;

namespace LedBlinker.LedToolkit.Tools.Impl
{
   public class ConfigurationStateTool : IConfigurationStateTool
    {
        private HttpClient _client;

        public ConfigurationStateTool(HttpClient client) => _client = client;

        public async Task<float?> LoadConfigurationState()
        {
            var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/configuration"));
            resultResponse.EnsureSuccessStatusCode();

            var resultMessage = await resultResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<float?>(resultMessage, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,


            }) ?? null;


        }

        public async Task<float?> SetConfigurationStateAsync(ConfigurationDto dto)
        {
            // Serializujeme DTO do JSON
            var json = JsonSerializer.Serialize(dto);

            // Vytvoříme HttpContent
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Pošleme POST na endpoint "/api/led/configuration"
            var response = await _client.PostAsync("/api/led/configuration", content);
            response.EnsureSuccessStatusCode();

            var resultMessage = await response.Content.ReadAsStringAsync(); // výsledek požadavku na controller

            return JsonSerializer.Deserialize<float?>(resultMessage, new JsonSerializerOptions() // 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
            }) ?? null;


        }
    }
}
