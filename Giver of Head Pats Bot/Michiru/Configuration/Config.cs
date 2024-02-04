using System.Text.Json;
using Michiru.Configuration.Classes;
using Serilog;

namespace Michiru.Configuration; 

public static class Config {
    public static Base Base { get; private set; }
    private static readonly ILogger Logger = Log.ForContext(typeof(Config));

    public static void Initialize() {
        const string file = "Michiru.Bot.config.json";
        var hasFile = File.Exists(file);
        
        var pennyGuildWatcher = new PennysGuildWatcher {
            GuildId = 0,
            ChannelId = 0,
            LastUpdateTime = 0
        };
        
        var rotatingStatus = new RotatingStatus {
            Enabled = false,
            Statuses = [
                new Status {
                    Id = 0,
                    ActivityText = "lots of cutes",
                    ActivityType = "Watching",
                    UserStatus = "Online"
                }
            ]
        };
        
        var personalizedMember = new PersonalizedMember {
            Guilds = []
        };

        var banger = new Banger {
            Enabled = false,
            GuildId = 0,
            ChannelId = 0,
            SubmittedBangers = 0,
            WhitelistedUrls = [ "open.spotify.com", "youtube.com", "www.youtube.com", "music.youtube.com", "youtu.be", "deezer.com", "tidal.com", "bandcamp.com", "music.apple.com", "soundcloud.com" ],
            WhitelistedFileExtensions = [ "mp3", "flac", "wav", "ogg", "m4a", "alac", "aac", "aiff", "wma" ],
            UrlErrorResponseMessage = "This URL is not whitelisted.",
            FileErrorResponseMessage = "This file type is not whitelisted.",
            SpeakFreely = false,
            AddUpvoteEmoji = true,
            AddDownvoteEmoji = false,
            UseCustomUpvoteEmoji = true,
            CustomUpvoteEmojiName = "upvote",
            CustomUpvoteEmojiId = 1201639290048872529,
            UseCustomDownvoteEmoji = false,
            CustomDownvoteEmojiName = "downvote",
            CustomDownvoteEmojiId = 1201639287972696166
        };

        var config = new Base {
            ConfigVersion = Vars.TargetConfigVersion,
            BotToken = "",
            ActivityType = "Watching",
            ActivityText = "lots of cuties",
            UserStatus = "Online",
            RotatingStatus = rotatingStatus,
            OwnerIds = new List<ulong>(),
            BotLogsChannel = 0,
            ErrorLogsChannel = 0,
            Banger = [ banger ],
            PersonalizedMember = [ personalizedMember ],
            PennysGuildWatcher = pennyGuildWatcher
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
    
    public static void Save() 
        => File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Michiru.Bot.config.json"), JsonSerializer.Serialize(Base, new JsonSerializerOptions {WriteIndented = true}));

    public static Banger GetGuildBanger(ulong id) {
        var banger = Base.Banger!.FirstOrDefault(b => b.GuildId == id);
        if (banger is not null) return banger;
        banger = new Banger {GuildId = id};
        Base.Banger!.Add(banger);
        Save();
        return banger;
    }

    public static PmGuildData GetGuildPersonalizedMember(ulong id) {
        var pm = Base.PersonalizedMember!.FirstOrDefault(p => p.Guilds!.Any(g => g.GuildId == id));
        if (pm is not null) return pm.Guilds!.First(g => g.GuildId == id);
        pm = new PersonalizedMember {Guilds = new List<PmGuildData> {new() {GuildId = id}}};
        Base.PersonalizedMember!.Add(pm);
        Save();
        return pm.Guilds!.First();
    }
    
    public static int GetBangerNumber() => Base.Banger.Sum(guild => guild.SubmittedBangers);

    public static int GetPersonalizedMemberCount() => Base.PersonalizedMember.SelectMany(member => member.Guilds!).Sum(guild => guild.Members!.Count);
}