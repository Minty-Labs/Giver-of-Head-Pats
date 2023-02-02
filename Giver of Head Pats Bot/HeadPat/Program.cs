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

public sealed class Program {
    public static DiscordClient? Client { get; set; }
    public static CommandsNextExtension? Commands { get; set; }
    public static SlashCommandsExtension? Slash { get; set; }
    public static NekoClient? NekoClient { get; set; }
    
    private static void Main(string[] args) {
        Vars.IsWindows = Environment.OSVersion.ToString().ToLower().Contains("windows");
        Console.Title = string.Format($"{Vars.Name} v{Vars.Version}");
        new Program().MainAsync().GetAwaiter().GetResult();
    }
    
    private Program() {
        Logger.ConsoleLogger();
        Logger.Log("Elly is an adorable cute floof, I love her very very very much!~");
    }

    private async Task MainAsync() {
        Logger.Log("Bot is starting . . .");
        
        if (!Vars.IsDebug)
            MobileManager.CreateMobilePatch();
        
        Client = new DiscordClient(new DiscordConfiguration {
            MessageCacheSize = 100,
#if DEBUG
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
#endif
#if !DEBUG
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.None,
#endif
            Token = Vars.Config.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            AutoReconnect = true
        });
            
        Logger.ReplaceDSharpLogs(Client);

        var serviceCollection = new ServiceCollection();

        var commandsNextConfiguration = new CommandsNextConfiguration {
            StringPrefixes = new[] { Vars.Config.Prefix!.ToLower(), Vars.Config.Prefix },
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

        NekoClient = new NekoClient(Vars.Name);
        
        eventHandler.Complete();

        await using var db = new Context();
        var check = db.Overall.AsQueryable()
            .Where(u => u.ApplicationId.Equals(Vars.ClientId)).ToList().FirstOrDefault();
        
        if (check == null) {
            var overall = new Overlord {
                ApplicationId = Vars.ClientId,
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
        Vars.StartTime = DateTime.Now;
        Vars.ThisProcess = Process.GetCurrentProcess();
        Logger.Log("Welcome, " + "Lily".Pastel("9fffe3"));
        Logger.Log("Bot Version                    = " + Vars.Version);
        Logger.Log("Process ID                     = " + Vars.ThisProcess.Id);
        Logger.Log("Build Date                     = " + Vars.BuildDateShort);
        Logger.Log("Current OS                     = " + (Vars.IsWindows ? "Windows" : "Linux"));
        Logger.Log("Token                          = " + OutputStringAsHidden(Vars.Config.Token!).Pastel("FBADBC"));
        Logger.Log("Prefix (non-Slash)             = " + $"{Vars.Config.Prefix}".Pastel("FBADBC"));
        Logger.Log("ActivityType                   = " + $"{Vars.Config.ActivityType}".Pastel("FBADBC"));
        Logger.Log("Game                           = " + $"{Vars.Config.Game}".Pastel("FBADBC"));
        Logger.Log("Streaming URL                  = " + $"{Vars.Config.StreamingUrl}".Pastel("FBADBC"));
        Logger.Log("Number of Commands (non-Slash) = " + $"{Commands?.RegisteredCommands.Count}".Pastel("FBADBC"));
        await Client!.UpdateStatusAsync(new DiscordActivity {
            Name = $"{Vars.Config.Game}",
            ActivityType = GetActivityType(Vars.Config.ActivityType!)
        },
#if !DEBUG
            UserStatus.Online
#endif
#if DEBUG
            UserStatus.Idle
#endif
            );

        Console.Title = string.Format($"{Vars.Name} v{Vars.Version} - {Vars.Config.Game}");
        Logger.WriteSeparator("C75450");

        await using var db = new Context();
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
        var em = new DiscordEmbedBuilder();
        em.WithColor(Vars.IsDebug ? DiscordColor.Yellow : DiscordColor.SpringGreen);
        em.WithDescription($"Bot has started on {(Vars.IsWindows ? "Windows" : "Linux")}\n" +
                           $"Currently in {sender.Guilds.Count} Guilds with {tempPatCount} total head pats given");
        em.AddField("Build Time", $"{Vars.BuildTime:F}\n<t:{TimeConverter.GetUnixTime(Vars.BuildTime)}:R>");
        em.AddField("Start Time", $"{DateTime.Now:F}\n<t:{TimeConverter.GetUnixTime(DateTime.Now)}:R>");
        em.AddField("DSharpPlus Version", Vars.DSharpVer);
        em.WithFooter($"v{Vars.Version}");
        em.WithTimestamp(DateTime.Now);
        GeneralLogChannel = await sender.GetChannelAsync(Vars.Config.GeneralLogChannelId);
        ErrorLogChannel = await sender.GetChannelAsync(Vars.Config.ErrorLogChannelId);
        MessageCreated.DmCategory = await sender.GetChannelAsync(Vars.Config.DmResponseCategoryId);
        TaskScheduler.StartStatusLoop();
        await AutoRemoveOldDmChannels.RemoveOldDmChannelsTask();
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
