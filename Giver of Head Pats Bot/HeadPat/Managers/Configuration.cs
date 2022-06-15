using Newtonsoft.Json;

namespace HeadPats.Managers;

public class Config {
    public string Token { get; set; } = "";
    [JsonProperty("OwnerID")] public ulong OwnerUserId { get; set; } = 0;
    public string Prefix { get; set; } = "-";
    public string ActivityType { get; set; } = "Watching";
    public string Game { get; set; } = "all the cuties";
    public string StreamingUrl { get; set; } = "";
    
    [JsonProperty("SupportGuildID")] public ulong SupportGuildId { get; set; } = 0;

    [JsonProperty("GeneralLogChannelID")] public ulong GeneralLogChannelId { get; set; } = 0;

    [JsonProperty("ErrorLogChannelID")] public ulong ErrorLogChannelId { get; set; } = 0;
    
    [JsonProperty("DMResponseCategoryID")] public ulong DmResponseCategoryId { get; set; } = 0;
}

public static class Configuration {
    public static Config TheConfig { get; internal set; } = Load();

    private static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json")) return;
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json", JsonConvert.SerializeObject(new Config {
            Token = "",
            OwnerUserId = 0,
            Prefix = "-",
            ActivityType = "Watching",
            Game = "all the cuties",
            StreamingUrl = "",
            SupportGuildId = 0,
            GeneralLogChannelId = 0,
            ErrorLogChannelId = 0,
            DmResponseCategoryId = 0
        }));
        Save();
    }

    private static Config Load() {
        CreateFile();
        var d = JsonConvert.DeserializeObject<Config>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json"));
        return d ?? throw new Exception();
    }
    
    public static void Save() => File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json",
        JsonConvert.SerializeObject(TheConfig, Formatting.Indented));
}
