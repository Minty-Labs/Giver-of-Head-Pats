using Newtonsoft.Json;
using Serilog;

namespace HeadPats.Data;

public class GuildInfo {
    public ulong Id { get; set; }
    public List<string> CommandsToBlock { get; set; }
}

public class BlacklistedGuilds {
    [JsonProperty("Guilds")] public List<GuildInfo>? Guilds { get; set; }
}

public static class BlacklistedCmdsGuilds {
    public static BlacklistedGuilds BlacklistedGuilds = Load();
    
    public static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGuilds.json"))
            return;
        var guild = new GuildInfo {
            Id = 0,
            CommandsToBlock = new List<string> { "pat" }
        };
        BlacklistedGuilds = new BlacklistedGuilds {
            Guilds = new List<GuildInfo> { guild }
        };
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGuilds.json",
            JsonConvert.SerializeObject(BlacklistedGuilds, Formatting.Indented));
        Log.Debug("Created JSON: BlacklistedGuilds");
        Save();
    }

    private static BlacklistedGuilds Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<BlacklistedGuilds>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGuilds.json"));
        return j ?? throw new Exception();
    }

    private static void Save() {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}BlacklistedGuilds.json",
            JsonConvert.SerializeObject(BlacklistedGuilds, Formatting.Indented));
        Log.Debug("Saved JSON: BlacklistedGuilds");
    }
    
    public static async Task AddBlacklist(DSharpPlus.SlashCommands.InteractionContext cc, string guildId, string commandsToBlock) {
        var ulongId = ulong.Parse(guildId);
        if (BlacklistedGuilds.Guilds!.Any(c => c.Id.Equals(ulongId))) {
            await cc.CreateResponseAsync("Guild is already blacklisted.", true);
            return;
        }
        var array = commandsToBlock.Split(",");
        var guildInfo = new GuildInfo {
            Id = ulongId,
            CommandsToBlock = array.ToList()
        };

        var guild = await cc.Client.GetGuildAsync(ulongId);
        BlacklistedGuilds.Guilds!.Add(guildInfo);
        await cc.CreateResponseAsync($"Added Guild to the blacklist: `{guild.Name} ({guildId})` with the following commands: `{commandsToBlock.Replace(",", ", ")}`");
        Save();
    }
    
    public static async Task RemoveBlacklist(DSharpPlus.SlashCommands.InteractionContext cc, string guildId) {
        var ulongId = ulong.Parse(guildId);
        if (BlacklistedGuilds.Guilds!.Any(c => !c.Id.Equals(ulongId))) {
            await cc.CreateResponseAsync("Guild is not blacklisted.", true);
            return;
        }
        var guildInfo = BlacklistedGuilds.Guilds!.First(c => c.Id.Equals(ulongId));

        var guild = await cc.Client.GetGuildAsync(ulongId);
        BlacklistedGuilds.Guilds!.Remove(guildInfo);
        await cc.CreateResponseAsync($"Removed everything from the blacklist for `{guild.Name} ({guildId})`");
        Save();
    }
}