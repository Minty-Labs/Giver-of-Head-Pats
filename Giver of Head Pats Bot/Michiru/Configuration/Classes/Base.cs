using System.Text.Json.Serialization;

namespace Michiru.Configuration.Classes;

public class Base {
    public int ConfigVersion { get; set; } = 1;
    [JsonPropertyName("Token")] public string? BotToken { get; set; } = "";
    [JsonPropertyName("Activity Type")] public string ActivityType { get; set; } = "Watching";
    [JsonPropertyName("Game")] public string? ActivityText { get; set; } = "lots of cuties";
    [JsonPropertyName("Online Status")] public string UserStatus { get; set; } = "Online";
    [JsonPropertyName("Rotating Status")] public RotatingStatus RotatingStatus { get; set; } = new();
    [JsonPropertyName("Owner IDs")] public List<ulong>? OwnerIds { get; set; } = [];
    [JsonPropertyName("Bot Logs Channel")] public ulong BotLogsChannel { get; set; } = 0;
    [JsonPropertyName("Error Logs Channel")] public ulong ErrorLogsChannel { get; set; } = 0;
    [JsonPropertyName("Banger System")] public List<Banger> Banger { get; set; } = [];
    [JsonPropertyName("Personalized Members")] public List<PersonalizedMember> PersonalizedMember { get; set; } = [];
    [JsonPropertyName("Penny's Guild Watcher")] public PennysGuildWatcher PennysGuildWatcher { get; set; } = new();
}