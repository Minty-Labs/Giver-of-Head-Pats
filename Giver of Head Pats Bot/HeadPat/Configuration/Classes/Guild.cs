using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class GuildParams {
    [JsonPropertyName("Guild Name")] public string? GuildName { get; set; }
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; set; }
    [JsonPropertyName("Blacklisted Commands")] public List<string>? BlacklistedCommands { get; set; }
    public List<Reply>? Replies { get; set; }
    public ulong DailyPatChannelId { get; set; }
    public List<DailyPat>? DailyPats { get; set; }
    [JsonPropertyName("IRL Quotes")] public IrlQuotes IrlQuotes { get; set; }
}