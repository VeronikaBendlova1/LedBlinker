using System.Text.Json;
using LedBlinker.LedToolkit.Models;

namespace LedBlinker.LedToolkit.Tools.Impl;

public class LedStateTool : IStateTool
{
    private HttpClient _client;

    public LedStateTool(HttpClient client)
    {
        _client = client;
    }

    public async Task<Led> LoadState()
    {
        var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/state"));
        resultResponse.EnsureSuccessStatusCode();

        var resultMessage = await resultResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        }) ?? throw new Exception("Deserialization failed");


    }
}
