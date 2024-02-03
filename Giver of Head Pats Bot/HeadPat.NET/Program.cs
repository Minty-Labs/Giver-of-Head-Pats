using Serilog;
using Serilog.Events; 
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using fluxpoint_sharp;
using HeadPats.Configuration;
using HeadPats.Cookie;
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
    private static readonly ILogger UtilLogger = Log.ForContext("SourceContext", "Util");
    
    public DiscordSocketClient Client { get; set; }
    private InteractionService GlobalInteractions { get; set; }
    private InteractionService MintyLabsInteractions { get; set; }
    // private CommandService Commands { get; set; }
    public FluxpointClient FluxpointClient { get; set; }
    public CookieClient CookieClient { get; set; }
    
    public SocketTextChannel? GeneralLogChannel { get; set; }
    public SocketTextChannel? ErrorLogChannel { get; set; }
    public SocketCategoryChannel? DmCategory { get; set; }
    
    private static List<EventModule> _eventModules = [];
    private static List<BasicModule> _basicModules = [];

    public static async Task Main(string[] args) {
        Vars.IsWindows = Environment.OSVersion.ToString().Contains("windows", StringComparison.CurrentCultureIgnoreCase);
        Console.Title = $"{Vars.Name} v{Vars.Version} | Starting...";
        Logger.Information($"{Vars.Name} Bot is starting . . .");
        await new Program().MainAsync();
    }

    private Program() {
        Instance = this;
        Log.Logger = 
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(
                    initialMinimumLevel: LogEventLevel.Debug))
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
            MessageCacheSize = 2000,
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
        });
        
        // Commands = new CommandService(new CommandServiceConfig {
        //     LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
        //     DefaultRunMode = Discord.Commands.RunMode.Async,
        //     CaseSensitiveCommands = false,
        //     ThrowOnError = true
        // });
        
        GlobalInteractions = new InteractionService(Client, new InteractionServiceConfig {
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = Discord.Interactions.RunMode.Async,
            ThrowOnError = true
        });
        
        MintyLabsInteractions = new InteractionService(Client, new InteractionServiceConfig {
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = Discord.Interactions.RunMode.Async,
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

        // var argPos = 0;
        // Client.MessageReceived += async arg => {
            // Don't process the command if it was a system message
            // if (arg is not SocketUserMessage message)
            //     return;

            // Create a number to track where the prefix ends and the command begins

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            // if (!message.HasStringPrefix(Vars.IsDebug ? ".." : Config.Base.Prefix, ref argPos) || message.Author.IsBot)
            //     return;

            // Create a WebSocket-based command context based on the message
            // var context = new SocketCommandContext(Client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            // await Commands.ExecuteAsync(
            //     context: context,
            //     argPos: argPos,
            //     services: null);
        // };

        Client.Ready += ClientOnReady;
        
        var serviceCollection = new ServiceCollection();
        
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
            await GlobalInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            await MintyLabsInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            iLogger.Debug("{0} ran a command in guild {1}", arg.User.Username, arg.GuildId);
        };
        
        #endregion
        
        foreach (var module in _basicModules) {
            await module.InitializeAsync();
            module.InitializeClient(Client);
        }
        
        // _eventModules.Add(new MessageReceived());
        // _eventModules.Add(new OnBotJoinOrLeave());
        _eventModules.Add(new UserLeft());
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
        
        Logger.Information("Bot finished initializing, logging in to Discord...");
        await Client.LoginAsync(TokenType.Bot, Config.Base.BotToken);
        await Client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task ClientOnReady() {
        var crLogger = Log.ForContext("SourceContext", "ClientReady");
        Vars.StartTime = DateTime.UtcNow;
        crLogger.Information("Bot Version        = " + Vars.Version);
        crLogger.Information("Process ID         = " + Environment.ProcessId);
        crLogger.Information("Build Date         = " + Vars.BuildDate);
        crLogger.Information("Current OS         = " + (Vars.IsWindows ? "Windows" : "Linux"));
        crLogger.Information("Token              = " + Config.Base.BotToken!.Redact());
        crLogger.Information("ActivityType       = " + $"{Config.Base.ActivityType}");
        crLogger.Information("Game               = " + $"{Config.Base.ActivityText}");
        crLogger.Information("Number of Commands = " + $"{GlobalInteractions.SlashCommands.Count + MintyLabsInteractions.SlashCommands.Count}");

        await Client.SetStatusAsync(Vars.IsDebug || Vars.IsWindows ? UserStatus.DoNotDisturb : UserStatus.Online);
        
        if (Vars.IsWindows) {
            var temp1 = Config.Base.ActivityText!.Equals("(insert game here)") || string.IsNullOrWhiteSpace(Config.Base.ActivityText!);
            Console.Title = $"{Vars.Name} v{Vars.Version} | Logged in as {Client.CurrentUser.Username} - " +
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
            Description = $"Bot has started on {(Vars.IsWindows ? "Windows" : "Linux")}\n" +
                          $"Currently in {Client.Guilds.Count} Guilds with {tempPatCount} total head pats given",
            Footer = new EmbedFooterBuilder {
                Text = $"v{Vars.Version}",
                IconUrl = Client.CurrentUser.GetAvatarUrl()
            },
            Timestamp = DateTime.Now
        }
            .AddField("Build Time", $"<t:{Vars.BuildTime.ToUniversalTime().GetSecondsFromUtcUnixTime()}:F>\n<t:{Vars.BuildTime.ToUniversalTime().GetSecondsFromUtcUnixTime()}:R>")
            .AddField("Start Time", $"<t:{DateTime.UtcNow.GetSecondsFromUtcUnixTime()}:F>\n<t:{DateTime.UtcNow.GetSecondsFromUtcUnixTime()}:R>")
            .AddField("Discord.NET Version", Vars.DNetVer)
            .Build();
        
        if (!Config.Base.ErrorLogsChannel.IsZero()) 
            ErrorLogChannel = GetChannel(Vars.SupportServerId, Config.Base.ErrorLogsChannel);
        if (!Config.Base.DmCategory.IsZero())
            DmCategory = GetCategory(Vars.SupportServerId, Config.Base.DmCategory);
        if (!Config.Base.BotLogsChannel.IsZero()) {
            GeneralLogChannel = GetChannel(Vars.SupportServerId, Config.Base.BotLogsChannel);
            await GeneralLogChannel!.SendMessageAsync(embed: startEmbed);
        }
        if (_eventModules.Count != 0) {
            foreach (var module in _eventModules) {
                await module.OnSessionCreatedTask();
                module.OnSessionCreated();
            }
        }
        else await DNetToConsole.SendErrorToLoggingChannelAsync("No Event Modules were found or loaded!!");
        
        await GlobalInteractions.RegisterCommandsGloballyAsync();
        crLogger.Information("Registered global slash commands.");

        try {
            await MintyLabsInteractions.RegisterCommandsToGuildAsync(Vars.SupportServerId);
            crLogger.Information("Registered Owner slash commands for {0} ({1}).", "Minty Labs",  Vars.SupportServerId);
        }
        catch (Exception e) {
            crLogger.Error("Failed to register Owner slash commands for guild {0}\n{err}", Vars.SupportServerId, e);
        }
        
        // await Task.Delay(TimeSpan.FromSeconds(5));
        // OnBotJoinOrLeave.DoNotRunOnStart = false;
    }

    public SocketTextChannel? GetChannel(ulong guildId, ulong id) {
        var guild = Client.GetGuild(guildId);
        if (guild is null) {
            UtilLogger.Error("Selected guild {guildId} does not exist!", guildId);
            return null;
        }
        if (guild.GetTextChannel(id) is { } channel) return channel;
        UtilLogger.Error("Selected channel {id} does not exist!", id);
        return null;
    }
    
    public SocketUser? GetUser(ulong id) {
        if (Client.GetUser(id) is { } user) return user;
        UtilLogger.Error("Selected user {id} does not exist!", id);
        return null;
    }
    
    public SocketGuild? GetGuild(ulong id) {
        if (Client.GetGuild(id) is { } guild) return guild;
        UtilLogger.Error("Selected guild {id} does not exist!", id);
        return null;
    }
    
    public SocketUser? GetGuildUser(ulong guildId, ulong userId) {
        var guild = Client.GetGuild(guildId);
        if (guild is null) {
            UtilLogger.Error("Selected guild {guildId} does not exist! <GetGuildUser>", guildId);
            return null;
        }
        if (guild.GetUser(userId) is { } user) return user;
        UtilLogger.Error("Selected user {userId} does not exist! <GetGuildUser>", userId);
        return null;
    }
    
    public SocketCategoryChannel? GetCategory(ulong guildId, ulong id) {
        var guild = Client.GetGuild(guildId);
        if (guild is null) {
            UtilLogger.Error("Selected guild {guildId} does not exist!", guildId);
            return null;
        }
        if (guild.GetCategoryChannel(id) is { } category) return category;
        UtilLogger.Error("Selected category {id} does not exist!", id);
        return null;
    }
}