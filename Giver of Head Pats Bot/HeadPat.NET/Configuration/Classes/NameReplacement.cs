using System.Text.Json.Serialization;
namespace HeadPats.Configuration.Classes;

public class NameReplacement {
    [JsonPropertyName("User ID")] public ulong UserId { get; init; }
    [JsonPropertyName("Before Name")] public string? BeforeName { get; init; }
    public string? Replacement { get; set; }
}