using Newtonsoft.Json;

namespace HeadPats.Managers;

public class Config {
    [JsonProperty("Token")] public string? Token { get; set; }
    
    [JsonProperty("OwnerID")] public ulong OwnerUserId { get; set; }
    
    [JsonProperty("Prefix")] public string? Prefix { get; set; }
    
    [JsonProperty("ActivityType")] public string? ActivityType { get; set; }
    
    [JsonProperty("Game")] public string? Game { get; set; }
    
    [JsonProperty("StreamingUrl")] public string? StreamingUrl { get; set; }
    
    [JsonProperty("SupportGuildID")] public ulong SupportGuildId { get; set; }

    [JsonProperty("GeneralLogChannelID")] public ulong GeneralLogChannelId { get; set; }

    [JsonProperty("ErrorLogChannelID")] public ulong ErrorLogChannelId { get; set; }
    
    [JsonProperty("DMResponseCategoryID")] public ulong DmResponseCategoryId { get; set; }
    
    [JsonProperty("Unsplash Access Key")] public string? UnsplashAccessKey { get; set; }
    
    [JsonProperty("Unsplash Secret Key")] public string? UnsplashSecretKey { get; set; }
    [JsonProperty("CookieClientAPI Key")] public string? CookieClientApiKey { get; set; }
    
    [JsonProperty("Full Blacklist of Guilds")] public List<ulong>? FullBlacklistOfGuilds { get; set; }
    
    /*[JsonProperty("EnableRotation")] public bool EnableRotation;
     
    [JsonProperty("RotatingStatuses")] public List<RotatingStatus>? RotatingStatuses;
}

public class RotatingStatus {
    [JsonProperty("ActivityType")] public string? ActivityType;
    [JsonProperty("OnlineType")] public string? OnlineType;
    [JsonProperty("Game")] public string? Game;*/
}

public static class Configuration {
    public static Config TheConfig { get; internal set; } = Load();
    public static bool IsRotatingStatusesEnabled;

    private static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json")) return;

        // var rS1 = new RotatingStatus {
        //     ActivityType = "Playing",
        //     OnlineType = "Online",
        //     Game = "with cats"
        // };
        // var rS2 = new RotatingStatus {
        //     ActivityType = "Watching",
        //     OnlineType = "Online",
        //     Game = "all the cuties"
        // };

        var conf = new Config {
            Token = "",
            OwnerUserId = 0,
            Prefix = "-",
            ActivityType = "Watching",
            Game = "all the cuties",
            StreamingUrl = "",
            SupportGuildId = 0,
            GeneralLogChannelId = 0,
            ErrorLogChannelId = 0,
            DmResponseCategoryId = 0,
            UnsplashAccessKey = "",
            UnsplashSecretKey = "",
            CookieClientApiKey = "",
            FullBlacklistOfGuilds = new List<ulong> { 0 }
            // EnableRotation = false,
            // RotatingStatuses = new List<RotatingStatus> { rS1, rS2 }
        };
        
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json", JsonConvert.SerializeObject(conf, Formatting.Indented));
        Save();
    }

    private static Config Load() {
        CreateFile();
        var d = JsonConvert.DeserializeObject<Config>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json"));
        // try { IsRotatingStatusesEnabled = d!.EnableRotation; } catch { /*silence*/ }
        return d ?? throw new Exception();
    }
    
    public static void Save() => File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json",
        JsonConvert.SerializeObject(TheConfig, Formatting.Indented));

    // public static RotatingStatus[] Statuses() => TheConfig.RotatingStatuses!.ToArray();
}
