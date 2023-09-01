using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class Banger {
    public bool Enabled { get; set; }
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; set; }
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; }
    [JsonPropertyName("Whitelisted Music URLs")] public List<string>? WhitelistedUrls { get; set; }
    [JsonPropertyName("Whitelisted Music File Extensions")] public List<string>? WhitelistedFileExtensions { get; set; }
    [JsonPropertyName("URL Error Response Message")] public string? UrlErrorResponseMessage { get; set; }
    [JsonPropertyName("File Error Response Message")] public string? FileErrorResponseMessage { get; set; }
}