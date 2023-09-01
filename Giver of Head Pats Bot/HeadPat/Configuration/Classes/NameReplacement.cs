using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class NameReplacement {
    [JsonPropertyName("User ID")] public ulong UserId { get; set; }
    [JsonPropertyName("Before Name")] public string? BeforeName { get; set; }
    public string? Replacement { get; set; }
}