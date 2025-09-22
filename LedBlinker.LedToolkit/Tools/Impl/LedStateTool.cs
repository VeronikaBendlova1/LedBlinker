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
        var resultResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/led/state")); // po�li po�adavek na metodu get "/api/led/state"
        resultResponse.EnsureSuccessStatusCode(); //odpov�d je 200OK? je, pokra�uje se d�l - nen� vyhod� HttpRequestException

        var resultMessage = await resultResponse.Content.ReadAsStringAsync(); // v�sledek response (po�adavku) - na�ti t�lo odpov�di pomoc� metody ReadAsStringAsync()
        return JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions() // 
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true, // umo��uje serializaci i ve�ejn�ch pol� tj. nejen vlastnost� s get a set
        }) ?? throw new Exception("Deserialization failed");


    }


    public async Task<Led> SetStateAsync(LedStateDto dto)
    {
        // Serializujeme DTO do JSON
        var json = JsonSerializer.Serialize(dto);

        // Vytvo��me HttpContent
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Po�leme POST na endpoint "/api/led/state"
        var response = await _client.PostAsync("/api/led/state", content);
        response.EnsureSuccessStatusCode();

        var resultMessage = await response.Content.ReadAsStringAsync(); // v�sledek po�adavku na controller

        return JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions() // 
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        }) ?? throw new Exception("Deserialization failed");
    }
        /* kdy� chce� vr�tit nejen objekt LED, ale i informaci o �sp�chu*
          public async Task<Result<Led>> SetStateAsync(LedStateDto dto)
         var led = JsonSerializer.Deserialize<Led>(resultMessage, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        }) ?? throw new Exception("Deserialization failed");

        return new Result<Led> { Success = true, Data = led }; */
}
