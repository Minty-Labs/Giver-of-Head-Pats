using System.Text.Json;
using HeadPats.Configuration.Classes;
using Serilog;

namespace HeadPats.Configuration; 

public static class Config {
    public static Base Base { get; internal set; }
    private static readonly ILogger Logger = Log.ForContext(typeof(Config));

    public static void Initialize() {
        const string file = "Configuration.json";
        var hasFile = File.Exists(file);
        
        var rotatingStatus = new RotatingStatus {
            Enabled = false,
            Statuses = [
                new Status {
                    Id = 0,
                    ActivityText = "for 5 years",
                    ActivityType = "Playing",
                    UserStatus = "Online"
                }
            ]
        };
        
        var nameReplacement = new NameReplacement {
            UserId = 0,
            BeforeName = "MintLily",
            Replacement = "Lily"
        };
        
        var guildParams = new GuildParams {
            GuildName = "Your Guild Name",
            GuildId = 0,
            BlacklistedCommands = new List<string>(),
            DailyPatChannelId = 0,
            DailyPats = new List<DailyPat>(),
            DataDeletionTime = 0
        };
        
        var contributor = new BotContributor {
            UserName = "MintLily",
            Info = "Main/Lead Developer Bot Owner/Creator"
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

        var config = new Base {
            ConfigVersion = Vars.TargetConfigVersion,
            BotToken = "",
            Prefix = "hp!",
            ActivityType = "Playing",
            ActivityText = "with Headpats",
            UserStatus = "Online",
            RotatingStatus = rotatingStatus,
            OwnerIds = new List<ulong>(),
            BotLogsChannel = 0,
            ErrorLogsChannel = 0,
            DmCategory = 0,
            FullBlacklistOfGuilds = new List<ulong>(),
            Api = api,
            Contributors = new List<BotContributor> { contributor },
            GuildSettings = new List<GuildParams> { guildParams },
            NameReplacements = new List<NameReplacement> { nameReplacement }
        };
        
        bool update;
        Base? baseConfig = null;
        if (hasFile) {
            var oldJson = File.ReadAllText(file);
            baseConfig = JsonSerializer.Deserialize<Base>(oldJson);
            if (baseConfig?.ConfigVersion == Vars.TargetConfigVersion) {
                Base = baseConfig;
                update = false;
            } else {
                update = true;
                baseConfig!.ConfigVersion = Vars.TargetConfigVersion;
            }
        } else {
            update = true;
        }
        
        var json = JsonSerializer.Serialize(baseConfig ?? config, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText(file, json);
        Logger.Information("{0} {1}", update ? "Updated" : hasFile ? "Loaded" : "Created", file);
        Base = baseConfig ?? config;
    }
    
    public static bool ShouldUpdateConfigFile { get; private set; }
    public static void Save() => ShouldUpdateConfigFile = true;

    public static void SaveFile() {
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(Base, new JsonSerializerOptions {WriteIndented = true}));
        ShouldUpdateConfigFile = false;
    }
    
    public static GuildParams? GuildSettings(ulong guildId) => Base.GuildSettings?.FirstOrDefault(x => x.GuildId == guildId) ?? null;
}