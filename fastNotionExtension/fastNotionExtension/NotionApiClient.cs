// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fastNotionExtension;

public class NotionApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _databaseId;

    public NotionApiClient(string apiToken, string databaseId)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.notion.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        _httpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");
        _databaseId = databaseId;
    }

    public async Task CreatePageAsync(string title)
    {
        var payload = new
        {
            parent = new { database_id = _databaseId },
            properties = new
            {
                タイトル = new
                {
                    title = new[]
                    {
                        new { text = new { content = title } }
                    }
                },
                タスク種別 = new
                {
                    select = new
                    {
                        name = "📝CmdPalメモ"
                    }
                }
            }
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("pages", content);
        response.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
