using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using HeadPats.Cookie;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Handlers.Events;
using HeadPats.Managers;
using HeadPats.Utils;
using fluxpoint_sharp;
using HeadPats.Commands.ContextMenu;
using HeadPats.Commands.Legacy;
using HeadPats.Commands.Slash;
using HeadPats.Configuration;
using HeadPats.Modules;
using Serilog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace HeadPats;

public sealed class Program {
    public static DiscordClient? Client { get; set; }
    // public static CommandsNextExtension? Commands { get; set; }
    public static SlashCommandsExtension? Slash { get; set; }
    public static FluxpointClient? FluxpointClient { get; set; }
    public static CookieClient? CookieClient { get; set; }
    
    private static List<EventModule> _eventModules = new();
    private static List<BasicModule> _basicModules = new();
    
    internal static DiscordChannel? GeneralLogChannel, ErrorLogChannel;
    public static bool FirstTimeRun;
    
    private static void Main(string[] args) {
        Vars.IsWindows = Environment.OSVersion.ToString().ToLower().Contains("windows");
        Console.Title = string.Format($"{Vars.Name} v{Vars.Version}");
        Log.Debug($"{Vars.Name} Bot is starting . . .");
        new Program().MainAsync().GetAwaiter().GetResult();
    }
    
    private Program() {
        Log.Logger = 
            new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "start_.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 25, 
                    rollOnFileSizeLimit: true, fileSizeLimitBytes: 1024000000L)
                .CreateLogger();
        Log.Information("Elly is an adorable cute floof, I love her very very very much!~");
    }

    private async Task MainAsync() {
        var configNeedsSaving = false;
        var tempToken = string.Empty;
        if (Vars.IsWindows && string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Console.Title = $"{Vars.Name} | Enter your bot token";
            Log.Information("Please enter your bot token:");
            tempToken = Console.ReadLine()!.Trim();
            configNeedsSaving = true;
        }
        else if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Log.Error("Cannot proceed without a bot token. Please enter your bot token in the Configuration.json file.");
            await Log.CloseAndFlushAsync();
            Environment.Exit(0);
        }
        
        if (Vars.IsWindows)
            Console.Title = $"{Vars.Name} | Loading...";
        
        _basicModules.Add(new DSharpToConsole());
        // _basicModules.Add(new Config());
        _basicModules.Add(new LoopingTaskScheduler());
        if (!Vars.IsDebug) {
            // _basicModules.Add(new MobileManager());
            MobileManager.Initialize();
        }

        Client = new DiscordClient(new DiscordConfiguration {
            MessageCacheSize = 100,
            MinimumLogLevel = Vars.IsDebug || Vars.IsWindows ? LogLevel.Debug : LogLevel.None,
            Token = Config.Base.BotToken,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds,// | DiscordIntents.MessageContents | DiscordIntents.DirectMessages,
            AutoReconnect = true
        });
        
        if (configNeedsSaving) {
            Config.Base.BotToken = tempToken;
            Config.Save();
        }

        var serviceCollection = new ServiceCollection();

        var commandsNextConfiguration = new CommandsNextConfiguration {
            // StringPrefixes = Vars.IsDebug ? new [] { ".." } : new[] { Config.Base.Prefix.ToLower(), Config.Base.Prefix },
            EnableMentionPrefix = true,
            EnableDefaultHelp = false
        };

        // Commands = Client.UseCommandsNext(commandsNextConfiguration);
        Slash = Client.UseSlashCommands();
        
        // Commands.SetHelpFormatter<HelpFormatter>();

        foreach (var module in _basicModules) {
            module.Initialize();
            module.InitializeClient(Client);
        }
        
        GeneralLogChannel = await Client.GetChannelAsync(Config.Base.BotLogsChannel);
        ErrorLogChannel =   await Client.GetChannelAsync(Config.Base.ErrorLogsChannel);
        
        #region Legacy Commands
        
        // LegacyCommandHandler.RegisterLegacyCommands(Commands);
        // Commands.CommandExecuted += (sender, args) => {
        //     Log.Information($"Command {args.Command.Name}, executed by {args.Context.User.Username}, " +
        //                     $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name}");
        //     return Task.CompletedTask;
        // };
        // Commands.CommandErrored += (sender, args) => {
        //     Log.Information($"Command {(args.Command != null ? args.Command.Name : "Unknown Command")}, executed by {args.Context.User.Username}, " +
        //                     $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name} failed with\n{args.Exception.Message}");
        //     return Task.CompletedTask;
        // };
        
        #endregion

        #region Slash Commands
        
        SlashCommandHandler.RegisterSlashCommands(Slash);
        ContextMenuHandler.RegisterSlashCommands(Slash);
        Slash.SlashCommandExecuted += (sender, args) => {
            Log.Information($"Slash Command {args.Context.CommandName}, executed by {args.Context.User.Username}, " +
                            $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name}");
            return Task.CompletedTask;
        };
        Slash.SlashCommandErrored += (sender, args) => {
            var message = $"Slash Command {args.Context.CommandName}, executed by {args.Context.User.Username}, " +
                          $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name} failed with:\n" +
                          $"Message:\n{args.Exception.Message}\n" +
                          $"StackTrace:\n{args.Exception.StackTrace}";
            Log.Information(message);
            DSharpToConsole.SendErrorToLoggingChannel(message);
            return Task.CompletedTask;
        };

        #endregion

        Client.SessionCreated += SessionCreated;
        _eventModules.Add(new BangerEventListener());
        _eventModules.Add(new MessageCreated());
        _eventModules.Add(new OnBotJoinOrLeave());
        _eventModules.Add(new OnMemberLeave());
        _eventModules.ForEach(module => module.Initialize(Client));
        
        if (!string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.CookieClientApiKey))
            CookieClient = new CookieClient(Config.Base.Api.ApiKeys.CookieClientApiKey!);
        
        if (!string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.FluxpointApiKey!))
            FluxpointClient = new FluxpointClient(Vars.Name, Config.Base.Api.ApiKeys.FluxpointApiKey!);

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
        
        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    private static async Task SessionCreated(DiscordClient sender, DSharpPlus.EventArgs.SessionReadyEventArgs e) {
        Vars.StartTime = DateTime.Now;
        Vars.ThisProcess = Process.GetCurrentProcess();
        Log.Debug("Bot Version        = " + Vars.Version);
        Log.Debug("Process ID         = " + Vars.ThisProcess.Id);
        Log.Debug("Build Date         = " + Vars.BuildDate);
        Log.Debug("Current OS         = " + (Vars.IsWindows ? "Windows" : "Linux"));
        Log.Debug("Token              = " + Config.Base.BotToken!.Redact());
        Log.Debug("Legacy Prefix      = " + $"{Config.Base.Prefix}");
        Log.Debug("ActivityType       = " + $"{Config.Base.ActivityType}");
        Log.Debug("Game               = " + $"{Config.Base.ActivityText}");
        Log.Debug("Online Status      = " + $"{Config.Base.UserStatus}");
        // Log.Debug("Number of Commands = " + $"{Commands?.RegisteredCommands.Count + Slash?.RegisteredCommands.Count} (:salute: thanks Discord)");

        if (Vars.IsWindows) {
            var temp1 = Config.Base.ActivityText!.Equals("(insert game here)") || string.IsNullOrWhiteSpace(Config.Base.ActivityText!);
            Console.Title = $"{Vars.Name} v{Vars.Version} | Logged in as {sender.CurrentUser.Username} - " +
                            $"Currently in {Client!.Guilds.Count} Guilds - " +
                            $"{Config.Base.ActivityType} {(temp1 ? "unset" : Config.Base.ActivityText)}";
        }

        await using var db = new Context();
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
        
        var startEmbed = new DiscordEmbedBuilder {
            Color = Vars.IsDebug || Vars.IsWindows ? DiscordColor.Yellow : DiscordColor.SpringGreen,
            Description = $"Bot has started on {(Vars.IsWindows ? "Windows" : "Linux")}\n" +
                          $"Currently in {sender.Guilds.Count} Guilds with {tempPatCount} total head pats given",
            Footer = new DiscordEmbedBuilder.EmbedFooter {
                Text = $"v{Vars.Version}",
                IconUrl = Client!.CurrentUser.AvatarUrl ?? "https://i.mintlily.lgbt/null.jpg"
            },
            Timestamp = DateTime.Now
        }
            .AddField("Build Time", $"<t:{Vars.BuildTime.ToUniversalTime().GetSecondsFromUtcUnixTime()}:F>\n<t:{Vars.BuildTime.ToUniversalTime().GetSecondsFromUtcUnixTime()}:R>")
            .AddField("Start Time", $"<t:{DateTime.UtcNow.GetSecondsFromUtcUnixTime()}:F>\n<t:{DateTime.UtcNow.GetSecondsFromUtcUnixTime()}:R>")
            .AddField("DSharpPlus Version", Vars.DSharpVer)
            .Build();
        
        if (_eventModules.Count != 0) {
            foreach (var module in _eventModules) {
                await module.OnSessionCreatedTask();
                module.OnSessionCreated();
            }
        }
        else await DSharpToConsole.SendErrorToLoggingChannelAsync("No Event Modules were found or loaded!!");
        FirstTimeRun = true;
        // await AutoRemoveOldDmChannels.RemoveOldDmChannelsTask();
        await sender.SendMessageAsync(GeneralLogChannel, startEmbed);
    }
}
