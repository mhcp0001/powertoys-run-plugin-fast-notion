using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.NotionMemo.Models
{
    public class NotionPageResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }
    }

    public class NotionErrorResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}