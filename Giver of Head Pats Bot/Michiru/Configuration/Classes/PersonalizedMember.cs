using System.Text.Json.Serialization;
namespace Michiru.Configuration.Classes;

public class PersonalizedMember {
    public List<PmGuildData>? Guilds { get; set; } = [];
}

public class PmGuildData {
    [JsonPropertyName("Enabled")] public bool Enabled { get; set; } = false;
    [JsonPropertyName("Guild ID")] public ulong GuildId { get; set; } = 0;
    [JsonPropertyName("Channel ID")] public ulong ChannelId { get; set; } = 0;
    [JsonPropertyName("Reset Timer")] public int ResetTimer { get; set; } = 30;
    [JsonPropertyName("Default Role ID")] public ulong DefaultRoleId { get; set; } = 0;
    [JsonPropertyName("Members")] public List<Member>? Members { get; set; } = [];
}

public class Member {
    [JsonPropertyName("User ID")] public ulong userId { get; set; }
    [JsonPropertyName("Role ID")] public ulong roleId { get; set; }
    [JsonPropertyName("Role Name")] public string? roleName { get; set; }
    [JsonPropertyName("Color Hex")] public string colorHex { get; set; }
    [JsonPropertyName("Last Updated")] public long epochTime { get; set; }
}