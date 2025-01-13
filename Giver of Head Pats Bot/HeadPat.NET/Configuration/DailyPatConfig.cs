using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace HeadPats.Configuration;

public class DPBase {
    public List<DailyPatGuild>? Guilds { get; init; }
}

public class DailyPatUser {
    [JsonPropertyName("User ID")] public ulong UserId { get; set; }
    [JsonPropertyName("Set Epoch Time")] public long SetEpochTime { get; set; }
}

public class DailyPatGuild {
    public ulong GuildId { get; set; }
    public ulong DailyPatChannelId { get; set; }
    public List<DailyPatUser>? Users { get; set; }
}

public class DailyPatConfig {
    public static DPBase? Base { get; internal set; }
    private static readonly ILogger Logger = Log.ForContext(typeof(DailyPatConfig));
    
    public static void Initialize() {
        const string file = "DailyPats.json";
        var hasFile = File.Exists(file);

        if (!hasFile) {
            var user = new DailyPatUser {
                UserId = 0,
                SetEpochTime = 0
            };
        
            var guild = new DailyPatGuild {
                GuildId = 0,
                DailyPatChannelId = 0,
                Users = [ user ]
            };
        
            var dailyPatList = new DPBase {
                Guilds = [ guild ]
            };
        
            Base = dailyPatList;
            Logger.Information("Created new DailyPats.json file.");
            Save();
            return;
        }
        
        var json = File.ReadAllText(file);
        Base = JsonSerializer.Deserialize<DPBase>(json);
        Logger.Information("Loaded DailyPats.json file.");
    }
    
    public static void Save() => File.WriteAllText("DailyPats.json", JsonSerializer.Serialize(Base, new JsonSerializerOptions { WriteIndented = true }));
}