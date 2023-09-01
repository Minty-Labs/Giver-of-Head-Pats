using System.Text.Json.Serialization;
using HeadPats.Commands.Slash.Contributors;
namespace HeadPats.Configuration.Classes;

public class PersonalizedMember {
    [JsonPropertyName("Enabled")] public bool Enabled { get; set; }
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; }
    [JsonPropertyName("Members")] public List<Member>? Members { get; set; }
}

public class Member {
    [JsonPropertyName("User ID")] public ulong userId { get; set; } = 0;
    [JsonPropertyName("Role ID")] public ulong roleId { get; set; } = 0;
    [JsonPropertyName("Role Name")] public string? roleName { get; set; } = "__{x@}";
    [JsonPropertyName("Color Hex")] public string colorHex { get; set; } = "000000";
    [JsonPropertyName("Last Updated")] public long epochTime { get; set; } = 0;
}