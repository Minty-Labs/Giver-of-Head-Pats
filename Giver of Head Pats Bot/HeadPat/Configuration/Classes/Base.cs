using System.Text.Json.Serialization;

namespace HeadPats.Configuration.Classes;

public class Base {
    [JsonPropertyName("Token")] public string? BotToken { get; set; }
    [JsonPropertyName("Non-Slash Prefix")] public string Prefix { get; set; } = "hp!";
    [JsonPropertyName("Activity Type")] public string ActivityType { get; set; } = "Playing";
    [JsonPropertyName("Game")] public string? ActivityText { get; set; }
    [JsonPropertyName("Online Status")] public string UserStatus { get; set; } = "Online";
    [JsonPropertyName("Rotating Status")] public RotatingStatus RotatingStatus { get; set; }
    [JsonPropertyName("Owner IDs")] public List<ulong>? OwnerIds { get; set; }
    [JsonPropertyName("Bot Logs Channel")] public ulong BotLogsChannel { get; set; }
    [JsonPropertyName("Error Logs Channel")] public ulong ErrorLogsChannel { get; set; }
    [JsonPropertyName("Direct Message Category ID")] public ulong DmCategory { get; set; }
    [JsonPropertyName("Full Blacklist of Guilds")] public List<ulong>? FullBlacklistOfGuilds { get; set; }
    [JsonPropertyName("APIs")] public Api Api { get; set; }
    [JsonPropertyName("Contributors")] public List<BotContributor> Contributors { get; set; }
    [JsonPropertyName("Guild Settings")] public List<GuildParams>? GuildSettings { get; set; }
    [JsonPropertyName("Name Replacements")] public List<NameReplacement>? NameReplacements { get; set; }
    [JsonPropertyName("Banger System")] public Banger? Banger { get; set; }
    [JsonPropertyName("Personalized Members")] public PersonalizedMember PersonalizedMemberLily { get; set; }
    [JsonPropertyName("Personalized Members for Penny")] public PersonalizedMember PersonalizedMemberPenny { get; set; }
}