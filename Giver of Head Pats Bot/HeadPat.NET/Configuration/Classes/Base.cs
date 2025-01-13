using System.Text.Json.Serialization;

namespace HeadPats.Configuration.Classes;

public class Base {
    public int ConfigVersion { get; set; }
    [JsonPropertyName("Token")] public string? BotToken { get; set; }
    [JsonPropertyName("Non-Slash Prefix")] public string Prefix { get; init; } = "hp!";
    [JsonPropertyName("Activity Type")] public string ActivityType { get; init; } = "Playing";
    [JsonPropertyName("Game")] public string? ActivityText { get; init; }
    [JsonPropertyName("Online Status")] public string UserStatus { get; init; } = "Online";
    [JsonPropertyName("Rotating Status")] public RotatingStatus RotatingStatus { get; init; }
    [JsonPropertyName("Owner IDs")] public List<ulong>? OwnerIds { get; init; }
    [JsonPropertyName("Bot Logs Channel")] public ulong BotLogsChannel { get; init; }
    [JsonPropertyName("Error Logs Channel")] public ulong ErrorLogsChannel { get; init; }
    [JsonPropertyName("Direct Message Category ID")] public ulong DmCategory { get; init; }
    [JsonPropertyName("Full Blacklist of Guilds")] public List<ulong>? FullBlacklistOfGuilds { get; init; }
    [JsonPropertyName("APIs")] public Api Api { get; init; }
    [JsonPropertyName("Contributors")] public List<BotContributor> Contributors { get; init; }
    [JsonPropertyName("Guild Settings")] public List<GuildParams>? GuildSettings { get; init; }
    [JsonPropertyName("Name Replacements")] public List<NameReplacement>? NameReplacements { get; init; }
    public string? LocalImagePath { get; set; }
}