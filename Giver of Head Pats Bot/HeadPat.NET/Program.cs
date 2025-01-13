using Serilog;
using Serilog.Events; 
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using fluxpoint_sharp;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using HeadPats.Commands.ContextMenu;
using HeadPats.Commands.Slash;
using HeadPats.Commands.Slash.Owner;
using HeadPats.Commands.Slash.UserLove;
using HeadPats.Commands.Slash.UserLove.Leaderboards;
using HeadPats.Events;
using HeadPats.Managers;
using HeadPats.Modules;
using Serilog.Core;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace HeadPats;

public class Program {
    public static Program Instance { get; private set; }
    private static readonly ILogger Logger = Log.ForContext("SourceContext", "GoHP");
    
    public DiscordSocketClient Client { get; private set; }
    private InteractionService GlobalInteractions { get; set; }
    private InteractionService MintyLabsInteractions { get; set; }
    
    public SocketTextChannel? GeneralLogChannel { get; private set; }
    public SocketTextChannel? ErrorLogChannel { get; private set; }
    // public SocketCategoryChannel? DmCategory { get; set; }
    
    private readonly List<EventModule> _eventModules = [];
    private readonly List<BasicModule> _basicModules = [];
    
    // private ModalProcessor _modalProcessor;
    public FluxpointClient FluxpointClient { get; private set; }

    public static async Task Main(string[] args) {
        Vars.IsWindows = Environment.OSVersion.ToString().Contains("windows", StringComparison.CurrentCultureIgnoreCase);
        Console.Title = $"{Vars.Name} v{Vars.VersionStr} | Starting...";
        Logger.Information($"{Vars.Name} Bot is starting . . .");
        await new Program().MainAsync();
    }

    private Program() {
        Instance = this;
        Log.Logger = 
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(
                    initialMinimumLevel: LogEventLevel.Information))
                .WriteTo.Console(new ExpressionTemplate(
                    template: "[{@t:HH:mm:ss} {@l:u3} {Coalesce(Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1),'unset')}] {@m}\n{@x}",
                    theme: TemplateTheme.Literate))
                .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "Logs", "start_.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 25, 
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 1024000000L)
                .CreateLogger();
    }

    private async Task MainAsync() {
        Config.Initialize();
        if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Console.Title = "HeadPat.NET | Enter your bot token";
            Logger.Information("Please enter your bot token:");
            Config.Base.BotToken = Console.ReadLine()!.Trim();
        }
        else if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Logger.Information("Cannot proceed without a bot token. Please enter your bot token in the Configuration.json file.");
            Environment.Exit(0);
        }
        if (Config.Base.BotLogsChannel.IsZero()) 
            Logger.Warning("Bot Logs Channel is not set. Please set the BotLogsChannel in the Configuration.json file.");
        if (Config.Base.ErrorLogsChannel.IsZero())
            Logger.Warning("Error Logs Channel is not set. Please set the ErrorLogsChannel in the Configuration.json file.");
        Config.Save();
        
        if (Vars.IsWindows)
            Console.Title = $"{Vars.Name} | Loading...";
        
        _basicModules.Add(new LoopingTaskScheduler());
        _basicModules.Add(new DNetToConsole());
        if (!Vars.IsDebug) 
            MobileManager.Initialize();
        
        Client = new DiscordSocketClient(new DiscordSocketConfig {
            AlwaysDownloadUsers = true,
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            MessageCacheSize = 1500,
            GatewayIntents = (GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged)
                             & ~GatewayIntents.GuildPresences & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites
        });
        
        GlobalInteractions = new InteractionService(Client, new InteractionServiceConfig {
            UseCompiledLambda = true,
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = RunMode.Async,
            ThrowOnError = true
        });
        
        MintyLabsInteractions = new InteractionService(Client, new InteractionServiceConfig {
            UseCompiledLambda = true,
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = RunMode.Async,
            ThrowOnError = true
        });
        
        Client.Log += msg => {
            var dnLogger = Log.ForContext("SourceContext", "DNET");
            var severity = msg.Severity switch {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            dnLogger.Write(severity, msg.Exception, "[{source}] {message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        };

        Client.Ready += ClientOnReady;
        // Client.ModalSubmitted += async arg => await ModalProcessor.ProcessModal(arg);
        
        var serviceCollection = new ServiceCollection();
        // _modalProcessor = new ModalProcessor();
        
        #region Slash Comands
        
        // Global
        await GlobalInteractions.AddModuleAsync<ContextMenuLove>(null);
        await GlobalInteractions.AddModuleAsync<Basic>(null);
        await GlobalInteractions.AddModuleAsync<ColorCmds>(null);
        await GlobalInteractions.AddModuleAsync<TopCookie>(null);
        await GlobalInteractions.AddModuleAsync<TopPat>(null);
        await GlobalInteractions.AddModuleAsync<Summon>(null);
        await GlobalInteractions.AddModuleAsync<Love>(null);
        await GlobalInteractions.AddModuleAsync<DailyPatCmds>(null);
        
        // Owner
        await MintyLabsInteractions.AddModuleAsync<Contributors>(null);
        await MintyLabsInteractions.AddModuleAsync<BotControl>(null);
        await MintyLabsInteractions.AddModuleAsync<ConfigControl>(null);

        Client.InteractionCreated += async arg => {
            var iLogger = Log.ForContext("SourceContext", "Interaction");
            try {
                await GlobalInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            }
            catch {
                if (arg.Type == InteractionType.ApplicationCommand) {
                    await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
                }
            }

            try {
                await MintyLabsInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            }
            catch {
                if (arg.Type == InteractionType.ApplicationCommand) {
                    await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
                }
            }
            
            iLogger.Debug("{0} ran a command in guild {1}", arg.User.Username, arg.GuildId);
        };
        
        #endregion
        
        foreach (var module in _basicModules) {
            await module.InitializeAsync();
            module.InitializeClient(Client);
        }
        
        // _eventModules.Add(new MessageReceived());
        _eventModules.Add(new OnBotJoinOrLeave());
        // _eventModules.Add(new UserLeft());
        _eventModules.ForEach(module => module.Initialize(Client));
        
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
        
        Logger.Information("Bot finished initializing, logging in to Discord...");
        await Client.LoginAsync(TokenType.Bot, Config.Base.BotToken);
        await Client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task ClientOnReady() {
        var crLogger = Log.ForContext("SourceContext", "ClientReady");
        Vars.StartTime = DateTime.UtcNow;
        crLogger.Information("Bot Version        = " + Vars.VersionStr);
        crLogger.Information("Process ID         = " + Environment.ProcessId);
        // crLogger.Information("Build Date         = " + Vars.BuildDate);
        crLogger.Information("Current OS         = " + (Vars.IsWindows ? "Windows" : "Linux"));
        crLogger.Information("Token              = " + Config.Base.BotToken!.Redact());
        crLogger.Information("ActivityType       = " + $"{Config.Base.ActivityType}");
        crLogger.Information("Game               = " + $"{Config.Base.ActivityText}");
        crLogger.Information("Rotating Statuses  = " + $"{Config.Base.RotatingStatus.Enabled}");
        if (Config.Base.RotatingStatus.Enabled)
            crLogger.Information("Statuses =         " + $"{string.Join(" | ", Config.Base.RotatingStatus.Statuses.Select(x => x.ActivityType + " - " + x.UserStatus + " - " + x.ActivityText).ToArray())}");
        crLogger.Information("Number of Commands = " + $"{GlobalInteractions.SlashCommands.Count + MintyLabsInteractions.SlashCommands.Count}");

        await Client.SetStatusAsync(Vars.IsDebug || Vars.IsWindows ? UserStatus.DoNotDisturb : UserStatus.Online);
        
        if (Vars.IsWindows) {
            var temp1 = Config.Base.ActivityText!.Equals("(insert game here)") || string.IsNullOrWhiteSpace(Config.Base.ActivityText!);
            Console.Title = $"{Vars.Name} v{Vars.VersionStr} | Logged in as {Client.CurrentUser.Username} - " +
                            $"Currently in {Client.Guilds.Count} Guilds - " +
                            $"{Config.Base.ActivityType} {(temp1 ? "unset" : Config.Base.ActivityText)}";
        }
        
        // OnBotJoinOrLeave.GuildIds = new List<ulong>();
        await using var db = new Context();
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
        
        // foreach (var guild in Client.Guilds) {
        //     OnBotJoinOrLeave.GuildIds.Add(guild.Id);
        // }

        var startEmbed = new EmbedBuilder {
            Color = Vars.IsDebug || Vars.IsWindows ? Colors.Yellow : Colors.HexToColor("9fffe3"),
            Footer = new EmbedFooterBuilder {
                Text = $"v{Vars.VersionStr}",
                IconUrl = Client.CurrentUser.GetAvatarUrl()
            },
            Timestamp = DateTime.Now
        }
            .AddField($"OS {(Vars.IsWindows ? "Windows" : "Linux")}", true)
            .AddField("Guilds", $"{Client.Guilds.Count:N0}", true)
            .AddField("Head Pats", $"{tempPatCount:N0}", true)
            // .AddField("Build Time", $"{Vars.BuildTime.ToUniversalTime().ConvertToDiscordTimestamp(TimestampFormat.LongDateTime)}\n{Vars.BuildTime.ToUniversalTime().ConvertToDiscordTimestamp(TimestampFormat.RelativeTime)}")
            .AddField("Start Time", $"{DateTime.UtcNow.ConvertToDiscordTimestamp(TimestampFormat.LongDateTime)}\n{DateTime.UtcNow.ConvertToDiscordTimestamp(TimestampFormat.RelativeTime)}")
            .AddField("Target .NET Version", Vars.TargetDotNetVersion, true)
            .AddField("System .NET Version", Environment.Version, true)
            .AddField("Discord.NET Version", Vars.DNetVer, true)
            .Build();
        
        if (!Config.Base.ErrorLogsChannel.IsZero()) 
            ErrorLogChannel = await Client.GetChannelAsync(Config.Base.ErrorLogsChannel) as SocketTextChannel;
        
        // if (!Config.Base.DmCategory.IsZero())
        //     DmCategory = await Client.GetChannelAsync(Config.Base.DmCategory) as SocketCategoryChannel;
        
        if (!Config.Base.BotLogsChannel.IsZero()) {
            GeneralLogChannel = await Client.GetChannelAsync(Config.Base.BotLogsChannel) as SocketTextChannel;
            await GeneralLogChannel!.SendMessageAsync(embed: startEmbed);
        }
        
        if (_eventModules.Count is not 0) {
            foreach (var module in _eventModules) {
                await module.OnSessionCreatedTask();
                module.OnSessionCreated();
            }
        }
        else await DNetToConsole.SendErrorToLoggingChannelAsync("Event Module Load Fail", obj: "No Event Modules were found or loaded!!");
        
        await GlobalInteractions.RegisterCommandsGloballyAsync();
        crLogger.Information("Registered global slash commands.");

        try {
            await MintyLabsInteractions.RegisterCommandsToGuildAsync(Vars.SupportServerId);
            crLogger.Information("Registered Owner slash commands for {0} ({1}).", "Minty Labs",  Vars.SupportServerId);
        }
        catch (Exception e) {
            crLogger.Error("Failed to register Owner slash commands for guild {0}\n{err}", Vars.SupportServerId, e);
        }
        
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetGameAsync("with love", type: ActivityType.Competing);
        
        await Task.Delay(TimeSpan.FromSeconds(5));
        OnBotJoinOrLeave.DoNotRunOnStart = false;
    }
}