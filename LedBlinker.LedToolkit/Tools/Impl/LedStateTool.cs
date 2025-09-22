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
        var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/state")); // pošli požadavek na metodu get "/api/led/state"
        resultResponse.EnsureSuccessStatusCode(); //odpovìd je 200OK? je, pokraèuje se dál - není vyhodí HttpRequestException

        var resultMessage = await resultResponse.Content.ReadAsStringAsync(); // výsledek response (požadavku) - naèti tìlo odpovìdi pomocí metody ReadAsStringAsync()
        return JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions() // 
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true, // umožòuje serializaci i veøejných polí tj. nejen vlastností s get a set
        }) ?? throw new Exception("Deserialization failed");


    }


    public async Task<Led> SetStateAsync(LedStateDto dto)
    {
        // Serializujeme DTO do JSON
        var json = JsonSerializer.Serialize(dto);

        // Vytvoøíme HttpContent
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Pošleme POST na endpoint "/api/led/state"
        var response = await _client.PostAsync("/api/led/state", content);
        response.EnsureSuccessStatusCode();

        var resultMessage = await response.Content.ReadAsStringAsync(); // výsledek požadavku na controller

        return JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions() // 
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        }) ?? throw new Exception("Deserialization failed");
    }
        /* když chceš vrátit nejen objekt LED, ale i informaci o úspìchu*
          public async Task<Result<Led>> SetStateAsync(LedStateDto dto)
         var led = JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        }) ?? throw new Exception("Deserialization failed");

        return new Result<Led> { Success = true, Data = led }; */
}
