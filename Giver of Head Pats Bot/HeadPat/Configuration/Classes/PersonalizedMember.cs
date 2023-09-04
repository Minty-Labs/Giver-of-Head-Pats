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

public static class PersonalizedMemberLogic {
    private static void FixPersonalizedMemberData() {
        if (Config.Base.PersonalizedMemberLily is null) {
            var personalization = new PersonalizedMember {
                Enabled = false,
                GuildId = 0,
                ChannelId = 0,
                ResetTimer = 30,
                DefaultRoleId = 0,
                Members = new List<Member>()
            };
            Config.Base.PersonalizedMemberLily = personalization;
        }

        if (Config.Base.PersonalizedMemberPenny is null) {
            var personalization = new PersonalizedMember {
                Enabled = false,
                GuildId = 0,
                ChannelId = 0,
                ResetTimer = 30,
                DefaultRoleId = 0,
                Members = new List<Member>()
            };
            Config.Base.PersonalizedMemberPenny = personalization;
        }
        Config.Save();
    }

    public static void SetLogicData() {
        FixPersonalizedMemberData(); // This will be removed in the future
        // ResetTimer = Config.Base.PersonalizedMemberLily.ResetTimer;
    }
}