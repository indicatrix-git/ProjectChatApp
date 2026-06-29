using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ChatClient.Models;
using Newtonsoft.Json;

namespace ChatClient.Services;

/// <summary>
/// Loads chat history from the REST API (GET /api/messages).
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<List<Message>> GetMessagesAsync()
    {
        var response = await _httpClient.GetAsync("/api/messages");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var messages = JsonConvert.DeserializeObject<List<Message>>(json);
        return messages ?? new List<Message>();
    }
}
