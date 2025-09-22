using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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


            }) ?? throw new Exception("Deserialization failed");


        }
    }
}
