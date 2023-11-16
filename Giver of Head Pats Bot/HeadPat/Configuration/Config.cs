using System.Text.Json;
using HeadPats.Configuration.Classes;

namespace HeadPats.Configuration; 

public static class Config {// : BasicModule {
    // protected override string ModuleName => "Configuration";
    // protected override string ModuleDescription => "Handles the configuration of the bot.";
    
    public static Base Base { get; internal set; } = Load();

    private static /*public override*/ void Initialize() {
        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Configuration.json"))) return;
        
        var rotatingStatus = new RotatingStatus {
            Enabled = false,
            Statuses = new List<Status> {
                new() {
                    Id = 0,
                    ActivityText = "for 5 years",
                    ActivityType = "Playing",
                    UserStatus = "Online"
                }
            }
        };

        var personalizationLily = new PersonalizedMember {
            Enabled = false,
            GuildId = 0,
            ChannelId = 0,
            ResetTimer = 30,
            DefaultRoleId = 0,
            Members = new List<Member>()
        };
        
        var personalizationPenny = new PersonalizedMember {
            Enabled = false,
            GuildId = 0,
            ChannelId = 0,
            ResetTimer = 30,
            DefaultRoleId = 0,
            Members = new List<Member>()
        };

        var banger = new Banger {
            Enabled = false,
            GuildId = 0,
            ChannelId = 0,
            WhitelistedUrls = new List<string> { "open.spotify.com", "youtube.com", "www.youtube.com", "music.youtube.com", "youtu.be", "deezer.com", "tidal.com", "bandcamp.com", "music.apple.com", "soundcloud.com" },
            WhitelistedFileExtensions = new List<string> { "mp3", "flac", "wav", "ogg", "m4a", "alac", "aac", "aiff", "wma" },
            UrlErrorResponseMessage = "This URL is not whitelisted.",
            FileErrorResponseMessage = "This file type is not whitelisted."
        };

        var irlq = new IrlQuotes {
            Enabled = false,
            ChannelId = 0,
            SetEpochTime = 0
        };
        
        var nameReplacement = new NameReplacement {
            UserId = 0,
            BeforeName = "MintLily",
            Replacement = "Lily"
        };

        // var reply = new Reply {
        //     Trigger = StringUtils.GetRandomString(),
        //     Response = StringUtils.GetRandomString(),
        //     OnlyTrigger = false,
        //     DeleteTrigger = false,
        //     DeleteTriggerIfIsOnlyInMessage = false
        // };
        
        var guildParams = new GuildParams {
            GuildName = "Your Guild Name",
            GuildId = 0,
            BlacklistedCommands = new List<string>(),
            // Replies = new List<Reply>(),
            DailyPatChannelId = 0,
            DailyPats = new List<DailyPat>(),
            IrlQuotes = irlq,
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
            NameReplacements = new List<NameReplacement> { nameReplacement },
            Banger = banger,
            PersonalizedMemberLily = personalizationLily,
            PersonalizedMemberPenny = personalizationPenny
        };
        
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(config, new JsonSerializerOptions {WriteIndented = true}));
    }
    
    private static Base Load() {
        Initialize();
        return JsonSerializer.Deserialize<Base>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"))) ?? throw new Exception();
    }
    
    public static void Save() 
        => File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(Base, new JsonSerializerOptions {WriteIndented = true}));
    
    public static GuildParams? GuildSettings(ulong guildId) => Base.GuildSettings?.FirstOrDefault(x => x.GuildId == guildId) ?? null;
    
    public static PersonalizedMember PersonalizedMember(ulong guildId) => Base.PersonalizedMemberLily.GuildId == guildId ? Base.PersonalizedMemberLily : Base.PersonalizedMemberPenny;
}