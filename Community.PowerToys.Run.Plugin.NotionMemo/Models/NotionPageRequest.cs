using System.Text.Json.Serialization;

namespace Community.PowerToys.Run.Plugin.NotionMemo.Models
{
    public class NotionPageRequest
    {
        [JsonPropertyName("parent")]
        public NotionParent Parent { get; set; }

        [JsonPropertyName("properties")]
        public NotionProperties Properties { get; set; }
    }

    public class NotionParent
    {
        [JsonPropertyName("database_id")]
        public string DatabaseId { get; set; }
    }

    public class NotionProperties
    {
        [JsonPropertyName("タイトル")]
        public NotionTitle Title { get; set; }

        [JsonPropertyName("タスク種別")]
        public NotionSelect TaskType { get; set; }
    }

    public class NotionTitle
    {
        [JsonPropertyName("title")]
        public NotionText[] TitleArray { get; set; }
    }

    public class NotionSelect
    {
        [JsonPropertyName("select")]
        public NotionSelectOption SelectOption { get; set; }
    }

    public class NotionText
    {
        [JsonPropertyName("text")]
        public NotionTextContent Text { get; set; }
    }

    public class NotionTextContent
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class NotionSelectOption
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}