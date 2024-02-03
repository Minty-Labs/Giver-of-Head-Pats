using System.Text.Json.Serialization;
namespace Michiru.Configuration.Classes;

public class Banger {
    public bool Enabled { get; set; }
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; set; }
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; }
    public int SubmittedBangers { get; set; } = 0;
    [JsonPropertyName("Whitelisted Music URLs")] public List<string>? WhitelistedUrls { get; set; }
    [JsonPropertyName("Whitelisted Music File Extensions")] public List<string>? WhitelistedFileExtensions { get; set; }
    [JsonPropertyName("URL Error Response Message")] public string? UrlErrorResponseMessage { get; set; }
    [JsonPropertyName("File Error Response Message")] public string? FileErrorResponseMessage { get; set; }
    public bool SpeakFreely { get; set; }
    public bool AddUpvoteEmoji { get; set; } = true;
    public bool AddDownvoteEmoji { get; set; } = false;
    public bool UseCustomUpvoteEmoji { get; set; } = true;
    public string CustomUpvoteEmojiName { get; set; } = "upvote";
    public ulong CustomUpvoteEmojiId { get; set; } = 1201639290048872529;
    public bool UseCustomDownvoteEmoji { get; set; } = false;
    public string CustomDownvoteEmojiName { get; set; } = "downvote";
    public ulong CustomDownvoteEmojiId { get; set; } = 1201639287972696166;
}