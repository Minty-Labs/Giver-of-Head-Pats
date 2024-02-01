using System.Text.Json.Serialization;
namespace HeadPats.Configuration.Classes;

public class Banger {
    public bool Enabled { get; set; }
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; set; }
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; }
    [JsonPropertyName("Whitelisted Music URLs")] public List<string>? WhitelistedUrls { get; set; }
    [JsonPropertyName("Whitelisted Music File Extensions")] public List<string>? WhitelistedFileExtensions { get; set; }
    [JsonPropertyName("URL Error Response Message")] public string? UrlErrorResponseMessage { get; set; }
    [JsonPropertyName("File Error Response Message")] public string? FileErrorResponseMessage { get; set; }
    public bool AddUpvoteEmoji { get; set; } = true;
    public bool AddDownvoteEmoji { get; set; } = false;
    public bool UseCustomUpvoteEmoji { get; set; } = true;
    public string CustomUpvoteEmojiName { get; set; } = "upvote";
    public ulong CustomUpvoteEmojiId { get; set; } = 1201639290048872529;
    public bool UseCustomDownvoteEmoji { get; set; } = false;
    public string CustomDownvoteEmojiName { get; set; } = "downvote";
    public ulong CustomDownvoteEmojiId { get; set; } = 1201639287972696166;
    public string NoticeComment { get; set; } = "Having a custom emoji ID of zero will logically mean that you are using a Discord default emoji.";
}