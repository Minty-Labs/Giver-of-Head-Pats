using System.Text.Json;
using HeadPats.Configuration.Classes;

namespace HeadPats.Configuration; 

public static class Config {
    public static Base Base { get; internal set; } = Load();

    public static void CreateFile() {
        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Configuration.json"))) return;

        var personalization = new PersonalizedMember {
            Enabled = false,
            ChannelId = 0,
            Members = new List<Member>()
        };

        var banger = new Banger {
            Enabled = false,
            GuildId = 0,
            ChannelId = 0,
            WhitelistedUrls = new List<string> { "open.spotify.com", "youtube.com", "www.youtube.com", "youtu.be", "deezer.com", "tidal.com", "bandcamp.com", "music.apple.com", "soundcloud.com" },
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
            Replies = new List<Reply>(),
            DailyPatChannelId = 0,
            DailyPats = new List<DailyPat>(),
            IrlQuotes = irlq
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
            PersonalizedMember = personalization
        };
        
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(config, new JsonSerializerOptions {WriteIndented = true}));
    }
    
    private static Base Load() {
        CreateFile();
        return JsonSerializer.Deserialize<Base>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"))) ?? throw new Exception();
    }
    
    public static void Save() 
        => File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Configuration.json"), JsonSerializer.Serialize(Base, new JsonSerializerOptions {WriteIndented = true}));
    
    public static GuildParams? GuildSettings(ulong guildId) => Base.GuildSettings?.FirstOrDefault(x => x.GuildId == guildId) ?? null;

    public static void FixPersonalizedMemberData() {
        if (Base.PersonalizedMember is not null) return;
        var personalization = new PersonalizedMember {
            Enabled = false,
            ChannelId = 0,
            Members = new List<Member>()
        };
        Base.PersonalizedMember = personalization;
        Save();
    }
}