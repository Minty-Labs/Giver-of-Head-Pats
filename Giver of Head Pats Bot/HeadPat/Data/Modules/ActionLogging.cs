/*using Newtonsoft.Json;

namespace HeadPats.Data.Modules;

public class Logic {
    public List<ActionsPerGuild>? ActionsPerGuilds { get; set; }
}

public class ActionsPerGuild {
    [JsonProperty("Guild ID")] public ulong GuildId { get; set; }
    public bool Enabled { get; set; }
    [JsonProperty("Log Channel Id")] public ulong LogChannelId { get; set; }
    public List<Actions>? Actions { get; set; }
}

public class Actions {
    [JsonProperty("Log Message Deletion")] public bool LogMessageDeletion { get; set; }
    [JsonProperty("Log Message Updates")] public bool LogMessageUpdates { get; set; }
    [JsonProperty("Log Name Updates")] public bool LogNameUpdates { get; set; }
    [JsonProperty("Log Voice Channel Joins and Leaves")] public bool LogVoiceChannelJoinLeave { get; set; }
    [JsonProperty("Log Bans and Unbans")] public bool LogBans { get; set; }
    [JsonProperty("Log Kicks")] public bool LogKicks { get; set; }
}


public class ActionLogging {
    public static Logic Base { get; set; } = Load();

    public static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}ActionLogging.json")) return;

        var a = new Actions {
            LogMessageDeletion = false,
            LogMessageUpdates = false,
            LogNameUpdates = false,
            LogVoiceChannelJoinLeave = false,
            LogBans = false,
            LogKicks = false
        };

        var g = new ActionsPerGuild {
            GuildId = 0,
            Enabled = false,
            LogChannelId = 0,
            Actions = new List<Actions> { a }
        };

        var l = new Logic {
            ActionsPerGuilds = new List<ActionsPerGuild> { g }
        };

        Base = l;
        
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}ActionLogging.json",
            JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Created JSON: ActionLogging");
        Save();
    }

    private static Logic Load() {
        CreateFile();
        var j = JsonConvert.DeserializeObject<Logic>(File.ReadAllText(
            $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}ActionLogging.json"));
        return j ?? throw new Exception();
    }
    
    private static void Save() {
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}ActionLogging.json",
            JsonConvert.SerializeObject(Base, Formatting.Indented));
        Logger.Log("Saved JSON: ActionLogging");
    }
}*/