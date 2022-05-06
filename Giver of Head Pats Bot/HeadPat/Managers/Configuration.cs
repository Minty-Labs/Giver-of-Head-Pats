using Newtonsoft.Json;

namespace HeadPats.Managers;

public class Config {
    public string Token { get; set; } = "";
    public string Prefix { get; set; } = "-";
    public string ActivityType { get; set; } = "Watching";
    public string Game { get; set; } = "the rain";
    public string StreamingUrl { get; set; } = "";
}

public static class Configuration {
    public static Config _conf { get; internal set; } = Load();
    public static readonly string ConfigFile = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json";

    private static void CreateFile() {
        if (File.Exists($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json")) return;
        File.WriteAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Config.json", JsonConvert.SerializeObject(new Config {
            Token = "",
            Prefix = "-",
            ActivityType = "Watching",
            Game = "the rain",
            StreamingUrl = ""
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
