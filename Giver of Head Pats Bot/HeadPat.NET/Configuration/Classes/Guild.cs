using System.Text.Json.Serialization;
namespace HeadPats.Configuration.Classes;

public class GuildParams {
    [JsonPropertyName("Guild Name")] public string? GuildName { get; init; }
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; init; }
    [JsonPropertyName("Blacklisted Commands")] public List<string>? BlacklistedCommands { get; init; }
    [JsonPropertyName("Data Deletion Time")] public long DataDeletionTime { get; set; }
}