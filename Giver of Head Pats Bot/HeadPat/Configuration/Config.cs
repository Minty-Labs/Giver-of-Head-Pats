using System.Text.Json;
using System.Text.Json.Serialization;
using HeadPats.Utils;

namespace HeadPats.Configuration; 

public class Config {
    [JsonPropertyName("Token")] public string? BotToken { get; set; }
    [JsonPropertyName("Non-Slash Prefix")] public string Prefix { get; set; } = "hp!";
    [JsonPropertyName("Activity Type")] public string ActivityType { get; set; } = "Playing";
    [JsonPropertyName("Game")] public string? Activity { get; set; }
    [JsonPropertyName("Streaming URL")] public string? StreamingUrl { get; set; }
    [JsonPropertyName("Owner IDs")] public List<ulong>? OwnerIds { get; set; }
    [JsonPropertyName("Bot Logs Channel")] public ulong? BotLogsChannel { get; set; }
    [JsonPropertyName("Error Logs Channel")] public ulong? ErrorLogsChannel { get; set; }
    [JsonPropertyName("Direct Message Category ID")] public ulong? DmCategory { get; set; }
    [JsonPropertyName("Full Blacklist of Guilds")] public List<ulong>? FullBlacklistOfGuilds { get; set; }
    [JsonPropertyName("APIs")] public Api? Api { get; set; }
    [JsonPropertyName("Contributors")] public List<Contributor>? Contributors { get; set; }
    [JsonPropertyName("Guild Settings")] public GuildParams? GuildSettings { get; set; }
}

public class Api {
    [JsonPropertyName("API Keys")] public ApiKeys? ApiKeys { get; set; }
    [JsonPropertyName("API Media URL Blacklist")] public List<string>? ApiMediaUrlBlacklist { get; set; }
}

public class ApiKeys {
    [JsonPropertyName("Unsplash Access Key")] public string? UnsplashAccessKey { get; set; }
    [JsonPropertyName("Unsplash Secret Key")] public string? UnsplashSecretKey { get; set; }
    [JsonPropertyName("CookieAPI Key")] public string? CookieClientApiKey { get; set; }
    [JsonPropertyName("FluxpointAPI Key")] public string? FluxpointApiKey { get; set; }
}

public class Contributor {
    public string? UserName { get; set; }
    public string? Info { get; set; }
}

public class GuildParams {
    [JsonPropertyName("Guild Name")] public string? GuildName { get; set; }
    [JsonPropertyName("Guild ID")] public ulong? GuildId { get; set; }
    [JsonPropertyName("Blacklisted Commands")] public List<string>? BlacklistedCommands { get; set; }
    public List<Reply>? Replies { get; set; }
}

public class Reply {
    public string? Trigger { get; set; }
    public string? Response { get; set; }
    [JsonPropertyName("Require Only Trigger Text")] public bool OnlyTrigger { get; set; }
    [JsonPropertyName("Delete Trigger")] public bool DeleteTrigger { get; set; }
    [JsonPropertyName("Delete Trigger If Is Only In Message")] public bool DeleteTriggerIfIsOnlyInMessage { get; set; }
}

public static class Configuration {
    public static Config Config { get; internal set; } = Load();

    public static void CreateFile() {
        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Configuration.json"))) return;

        var reply = new Reply {
            Trigger = StringUtils.GetRandomString(),
            Response = StringUtils.GetRandomString(),
            OnlyTrigger = false,
            DeleteTrigger = false,
            DeleteTriggerIfIsOnlyInMessage = false
        };
        
        var guildParams = new GuildParams {
            GuildName = "Your Guild Name",
            GuildId = 0,
            BlacklistedCommands = new List<string>(),
            Replies = new List<Reply> {reply}
        };
        
        var contributor = new Contributor {
            UserName = "Your Name",
            Info = "Your Info"
        };

        var apiKeys = new ApiKeys {
            UnsplashAccessKey = "",
            UnsplashSecretKey = "",
            CookieClientApiKey = "",
            FluxpointApiKey = ""
        };
        
        var api = new Api {
            ApiKeys = apiKeys,
            ApiMediaUrlBlacklist = new List<string>()
        };

        var config = new Config {
            BotToken = "",
            Prefix = "hp!",
            ActivityType = "Playing",
            Activity = "with Headpats",
            StreamingUrl = "",
            OwnerIds = new List<ulong>(),
            BotLogsChannel = 0,
            ErrorLogsChannel = 0,
            DmCategory = 0,
            FullBlacklistOfGuilds = new List<ulong>(),
            Api = api,
            Contributors = new List<Contributor> { contributor },
            GuildSettings = guildParams
        };
        
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(config, new JsonSerializerOptions {WriteIndented = true}));
    }
    
    private static Config Load() {
        CreateFile();
        return JsonSerializer.Deserialize<Config>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"))) ?? throw new Exception();
    }
    
    public static void Save() 
        => File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(Config, new JsonSerializerOptions {WriteIndented = true}));
}