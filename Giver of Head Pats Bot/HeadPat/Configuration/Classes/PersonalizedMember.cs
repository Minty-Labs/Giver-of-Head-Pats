using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class PersonalizedMember {
    [JsonPropertyName("Enabled")] public bool Enabled { get; set; }
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; set; }
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; }
    [JsonPropertyName("Reset Timer")] public int ResetTimer { get; set; } = 30;
    [JsonPropertyName("Default Role ID")] public ulong DefaultRoleId { get; set; }
    [JsonPropertyName("Members")] public List<Member>? Members { get; set; }
}

public class Member {
    [JsonPropertyName("User ID")] public ulong userId { get; set; }
    [JsonPropertyName("Role ID")] public ulong roleId { get; set; }
    [JsonPropertyName("Role Name")] public string? roleName { get; set; }
    [JsonPropertyName("Color Hex")] public string colorHex { get; set; }
    [JsonPropertyName("Last Updated")] public long epochTime { get; set; }
}