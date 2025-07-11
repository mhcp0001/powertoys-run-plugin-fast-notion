using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Community.PowerToys.Run.Plugin.NotionMemo.Models;

namespace Community.PowerToys.Run.Plugin.NotionMemo
{
    public class NotionApiClient : IDisposable
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string NotionApiVersion = "2022-06-28";
        private const string NotionApiBaseUrl = "https://api.notion.com/v1";

        static NotionApiClient()
        {
            _httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionApiVersion);
        }

        public async Task<NotionPageResponse> CreatePageAsync(string apiToken, string databaseId, string title, string taskType = "📝PowerToysメモ")
        {
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                throw new ArgumentException("API token is required", nameof(apiToken));
            }

            if (string.IsNullOrWhiteSpace(databaseId))
            {
                throw new ArgumentException("Database ID is required", nameof(databaseId));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required", nameof(title));
            }

            var request = new NotionPageRequest
            {
                Parent = new NotionParent
                {
                    DatabaseId = databaseId
                },
                Properties = new NotionProperties
                {
                    Title = new NotionTitle
                    {
                        TitleArray = new NotionText[]
                        {
                            new NotionText
                            {
                                Text = new NotionTextContent
                                {
                                    Content = title
                                }
                            }
                        }
                    },
                    TaskType = new NotionSelect
                    {
                        SelectOption = new NotionSelectOption
                        {
                            Name = taskType
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{NotionApiBaseUrl}/pages");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            httpRequestMessage.Content = content;

            try
            {
                var response = await _httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NotionPageResponse>(responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    NotionErrorResponse errorResponse = null;

                    try
                    {
                        errorResponse = JsonSerializer.Deserialize<NotionErrorResponse>(errorContent);
                    }
                    catch
                    {
                        // JSON parsing failed, use raw error content
                    }

                    throw new NotionApiException($"API request failed with status {response.StatusCode}: {errorResponse?.Message ?? errorContent}", response.StatusCode, errorResponse);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new NotionApiException($"Network error occurred: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new NotionApiException($"Request timeout: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            // HttpClient is static and shared, no need to dispose
        }
    }

    public class NotionApiException : Exception
    {
        public System.Net.HttpStatusCode? StatusCode { get; }
        public NotionErrorResponse NotionError { get; }

        public NotionApiException(string message) : base(message)
        {
        }

        public NotionApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotionApiException(string message, System.Net.HttpStatusCode statusCode, NotionErrorResponse notionError) : base(message)
        {
            StatusCode = statusCode;
            NotionError = notionError;
        }

        public string GetUserFriendlyMessage()
        {
            if (StatusCode.HasValue)
            {
                return StatusCode.Value switch
                {
                    System.Net.HttpStatusCode.Unauthorized => "エラー: Notion APIトークンが無効です。設定を確認してください。",
                    System.Net.HttpStatusCode.NotFound => "エラー: 指定されたデータベースが見つかりません。データベースIDを確認してください。",
                    System.Net.HttpStatusCode.BadRequest => "エラー: リクエストが不正です。データベースの構成を確認してください。",
                    System.Net.HttpStatusCode.TooManyRequests => "エラー: API利用制限に達しました。しばらく待ってから再試行してください。",
                    _ => $"エラー: Notion API接続に失敗しました ({StatusCode})。"
                };
            }
            else
            {
                return "エラー: ネットワーク接続を確認してください。";
            }
        }
    }
}