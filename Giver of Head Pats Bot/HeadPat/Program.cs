using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Data.Models;
// using HeadPats.Data.Modules;
using HeadPats.Handlers.Events;
using HeadPats.Managers;
using HeadPats.Utils;
using NekosSharp;
using Pastel;
using TaskScheduler = HeadPats.Managers.TaskScheduler;

namespace HeadPats;

public static class BuildInfo {
    public const string DSharpVer = "4.3.0-stable";
    public const string Name = "Giver of Head Pats";
    public const ulong ClientId = 489144212911030304;
    public const ulong TestGuildId = 279459962843955201;
#if DEBUG
    public const string Version = "4.5.0-dev2";
    public static readonly DateTime BuildTime = DateTime.Now;
    public static bool IsDebug = true;
#elif !DEBUG
    public const string Version = "4.10.1";
    public static readonly DateTime BuildTime = new(2023, 1, 30, 19, 54, 00); // (year, month, day, hour, min, sec)
    public static bool IsDebug = false;
#endif
    public static string BuildDateShort = $"{BuildTime.Day} {GetMonth(BuildTime.Month)} @ {BuildTime.Hour}:{ChangeSingleNumber(BuildTime.Minute)}";
    public static string BuildDate = $"Last Updated: {BuildDateShort}";
    public static DateTime StartTime = new();
    public static bool IsWindows;
    
    private static string GetMonth(int month) {
        return month switch {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => ""
        };
    }

    private static string ChangeSingleNumber(int num) {
        return num switch {
            0 => "00",
            1 => "01",
            2 => "02",
            3 => "03",
            4 => "04",
            5 => "05",
            6 => "06",
            7 => "07",
            8 => "08",
            9 => "09",
            _ => num.ToString()
        };
    }
        
    public static readonly Config Config = Configuration.TheConfig;
    public static Process? ThisProcess { get; set; }
}

public sealed class Program {
    public static DiscordClient? Client { get; set; }
    public static CommandsNextExtension? Commands { get; set; }
    public static SlashCommandsExtension? Slash { get; set; }
    public static NekoClient? NekoClient { get; set; }
    
    private static void Main(string[] args) {
        BuildInfo.IsWindows = Environment.OSVersion.ToString().ToLower().Contains("windows");
        Console.Title = string.Format($"{BuildInfo.Name} v{BuildInfo.Version}");
        new Program().MainAsync().GetAwaiter().GetResult();
    }
    
    private Program() {
        Logger.ConsoleLogger();
        Logger.Log("Elly is an adorable cute floof, I love her very very very much!~");
    }

    private async Task MainAsync() {
        Logger.Log("Bot is starting . . .");
        
        if (!BuildInfo.IsDebug)
            MobileManager.CreateMobilePatch();
        
        Client = new DiscordClient(new DiscordConfiguration {
            MessageCacheSize = 100,
#if DEBUG
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
#endif
#if !DEBUG
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.None,
#endif
            Token = BuildInfo.Config.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            AutoReconnect = true
        });
            
        Logger.ReplaceDSharpLogs(Client);

        var serviceCollection = new ServiceCollection();

        var commandsNextConfiguration = new CommandsNextConfiguration {
            StringPrefixes = new[] { BuildInfo.Config.Prefix!.ToLower(), BuildInfo.Config.Prefix },
            EnableDefaultHelp = true
        };

        Commands = Client.UseCommandsNext(commandsNextConfiguration);
        Slash = Client.UseSlashCommands();
        
        Commands.SetHelpFormatter<HelpFormatter>();

        Managers.Commands.Register(Commands);
        Commands.CommandExecuted += Commands_CommandExecuted;
        Commands.CommandErrored += Commands_CommandErrored;

        Managers.Commands.Register(Slash);
        Slash.SlashCommandErrored += Slash_SlashCommandErrored;

        Client.Ready += Client_Ready;
        var eventHandler = new Handlers.EventHandler(Client); // Setup Command Handler

        NekoClient = new NekoClient(BuildInfo.Name);
        
        eventHandler.Complete();

        await using var db = new Context();
        var check = db.Overall.AsQueryable()
            .Where(u => u.ApplicationId.Equals(BuildInfo.ClientId)).ToList().FirstOrDefault();
        
        if (check == null) {
            var overall = new Overlord {
                ApplicationId = BuildInfo.ClientId,
                PatCount = 0,
                NsfwCommandsUsed = 0
            };
            db.Overall.Add(overall);
            await db.SaveChangesAsync();
        }

        Client.UseInteractivity(new InteractivityConfiguration {
            PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.Ignore,
            Timeout = TimeSpan.FromMinutes(2)
        });

        /*await using var moderationDb = new ModerationModuleContext();
        var checkModerationModule = moderationDb.Moderation.AsQueryable().Where(m => m.GuildId == BuildInfo.TestGuildId).ToList().FirstOrDefault();
        
        if (checkModerationModule == null) {
            var moderation = new Moderation {
                GuildId = BuildInfo.TestGuildId, // Test Guild ID
                Enabled = false,
                LogChannelId = 1009952339832078396, // Test Log Channel ID
            };
            moderationDb.Add(moderation);
            await moderationDb.SaveChangesAsync();
        }

        GuildCommandCheckList = new List<GuildCommandCheck>();
        var things = new GuildCommandCheck {
            GuildId = BuildInfo.TestGuildId,
            Enabled = false
        };
        GuildCommandCheckList.Add(things);*/
        
        ReplyStructure.CreateFile();
        // MelonLoaderBlacklist.ProtectStructure.CreateFile();
        BlacklistedNekosLifeGifs.CreateFile();
        // ActionLogging.CreateFile();
            
        await Client.ConnectAsync();

        await Task.Delay(-1);
    }
    
    // public static List<GuildCommandCheck>? GuildCommandCheckList;
    
    internal static DiscordChannel? GeneralLogChannel, ErrorLogChannel;

    private static async Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e) {
        BuildInfo.StartTime = DateTime.Now;
        BuildInfo.ThisProcess = Process.GetCurrentProcess();
        Logger.Log("Welcome, " + "Lily".Pastel("9fffe3"));
        Logger.Log("Bot Version                    = " + BuildInfo.Version);
        Logger.Log("Process ID                     = " + BuildInfo.ThisProcess.Id);
        Logger.Log("Build Date                     = " + BuildInfo.BuildDateShort);
        Logger.Log("Current OS                     = " + (BuildInfo.IsWindows ? "Windows" : "Linux"));
        Logger.Log("Token                          = " + OutputStringAsHidden(BuildInfo.Config.Token!).Pastel("FBADBC"));
        Logger.Log("Prefix                         = " + $"{BuildInfo.Config.Prefix}".Pastel("FBADBC"));
        Logger.Log("ActivityType                   = " + $"{BuildInfo.Config.ActivityType}".Pastel("FBADBC"));
        Logger.Log("Game                           = " + $"{BuildInfo.Config.Game}".Pastel("FBADBC"));
        Logger.Log("Streaming URL                  = " + $"{BuildInfo.Config.StreamingUrl}".Pastel("FBADBC"));
        Logger.Log("Number of Commands (non-Slash) = " + $"{Commands?.RegisteredCommands.Count}".Pastel("FBADBC"));
        await Client!.UpdateStatusAsync(new DiscordActivity {
            Name = $"{BuildInfo.Config.Game}",
            ActivityType = GetActivityType(BuildInfo.Config.ActivityType!)
        },
#if !DEBUG
            UserStatus.Online
#endif
#if DEBUG
            UserStatus.Idle
#endif
            );

        Console.Title = string.Format($"{BuildInfo.Name} v{BuildInfo.Version} - {BuildInfo.Config.Game}");
        Logger.WriteSeparator("C75450");

        await using var db = new Context();
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
        var em = new DiscordEmbedBuilder();
        em.WithColor(BuildInfo.IsDebug ? DiscordColor.Yellow : DiscordColor.SpringGreen);
        em.WithDescription($"Bot has started on {(BuildInfo.IsWindows ? "Windows" : "Linux")}\n" +
                           $"Currently in {sender.Guilds.Count} Guilds with {tempPatCount} total head pats given");
        em.AddField("Build Time", $"{BuildInfo.BuildTime:F}\n<t:{TimeConverter.GetUnixTime(BuildInfo.BuildTime)}:R>");
        em.AddField("Start Time", $"{DateTime.Now:F}\n<t:{TimeConverter.GetUnixTime(DateTime.Now)}:R>");
        em.AddField("DSharpPlus Version", BuildInfo.DSharpVer);
        em.WithFooter($"v{BuildInfo.Version}");
        em.WithTimestamp(DateTime.Now);
        GeneralLogChannel = await sender.GetChannelAsync(BuildInfo.Config.GeneralLogChannelId);
        ErrorLogChannel = await sender.GetChannelAsync(BuildInfo.Config.ErrorLogChannelId);
        MessageCreated.DmCategory = await sender.GetChannelAsync(BuildInfo.Config.DmResponseCategoryId);
        TaskScheduler.StartStatusLoop();
        await sender.SendMessageAsync(GeneralLogChannel, em.Build());
    }

    private static Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e) {
        Logger.CommandExecuted(e.Command.Name, e.Context.Message.Author.Username, e.Context.Channel.IsPrivate ? "Direct Messages" : e.Context.Guild.Name);
        return Task.CompletedTask;
    }

    private static Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e) {
        if (e.Command == null && e.Context.Member != null)
            Logger.CommandNull(e.Context.Member.Username, e.Context.Message.Content);
        else
            Logger.CommandErrored(e.Command!.Name, e.Context.Message.Author.Username, e.Context.Channel.IsPrivate ? "Direct Messages" : e.Context.Guild.Name, e.Context.Message.Content, e.Exception);
        return Task.CompletedTask;
    }

    private static Task Slash_SlashCommandErrored(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs e) {
        Logger.SlashCommandErrored(e.Context.CommandName, e.Context.User.Username, e.Context.Channel.IsPrivate ? "Direct Messages" : e.Context.Guild.Name, e.Exception);
        return Task.CompletedTask;
    }
    
    private static string OutputStringAsHidden(string s) {
        string? temp = null;
        for (var i = 0; i < s.Length; i++)
            temp += "*";
        return temp ?? "***************";
    }
    
    private static ActivityType GetActivityType(string type) {
        return type.ToLower() switch {
            "playing" => ActivityType.Playing,
            "listening" => ActivityType.ListeningTo,
            "watching" => ActivityType.Watching,
            "streaming" => ActivityType.Streaming,
            "competing" => ActivityType.Competing,
            "play" => ActivityType.Playing,
            "listen" => ActivityType.ListeningTo,
            "watch" => ActivityType.Watching,
            "stream" => ActivityType.Streaming,
            "other" => ActivityType.Custom,
            "compete" => ActivityType.Competing,
            _ => ActivityType.Custom
        };
    }
    
    public static string GetActivityAsString(ActivityType type) {
        return type switch {
            ActivityType.Playing => "playing",
            ActivityType.ListeningTo => "listening",
            ActivityType.Watching => "watching",
            ActivityType.Streaming => "streaming",
            ActivityType.Competing => "competing",
            ActivityType.Custom => "other",
            _ => ""
        };
    }

    public static UserStatus GetUserStatus(string status) {
        return status.ToLower() switch {
            "online" => UserStatus.Online,
            "idle" => UserStatus.Idle,
            "donotdisturb" => UserStatus.DoNotDisturb,
            "dnd" => UserStatus.DoNotDisturb,
            "offline" => UserStatus.Offline,
            "invisible" => UserStatus.Invisible,
            "invis" => UserStatus.Invisible,
            _ => UserStatus.Online
        };
    }
}
