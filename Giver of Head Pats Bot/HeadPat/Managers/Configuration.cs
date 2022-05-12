using Newtonsoft.Json;

namespace HeadPats.Managers;

public class Config {
    public string Token { get; set; } = "";
    [JsonProperty("OwnerUserID")] public ulong OwnerUserId { get; set; } = 0;
    public string Prefix { get; set; } = "-";
    public string ActivityType { get; set; } = "Watching";
    public string Game { get; set; } = "all the cuties";
    public string StreamingUrl { get; set; } = "";

    [JsonProperty("GeneralLogChannelID")] public ulong GeneralLogChannelId { get; set; } = 0;

    [JsonProperty("ErrorLogChannelID")] public ulong ErrorLogChannelId { get; set; } = 0;
}

public static class Configuration {
    public static Config _conf { get; internal set; } = Load();
    public static readonly string ConfigFile = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json";

    private static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json")) return;
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json", JsonConvert.SerializeObject(new Config {
            Token = "",
            OwnerUserId = 0,
            Prefix = "-",
            ActivityType = "Watching",
            Game = "all the cuties",
            StreamingUrl = "",
            GeneralLogChannelId = 0,
            ErrorLogChannelId = 0
        }));
        Save();
    }

    private static Config Load() {
        CreateFile();
        var d = JsonConvert.DeserializeObject<Config>(File.ReadAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json"));
        return d ?? throw new Exception();
    }
    
    public static void Save() => File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json",
        JsonConvert.SerializeObject(_conf, Formatting.Indented));
}
