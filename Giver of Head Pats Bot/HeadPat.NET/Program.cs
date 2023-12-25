using Serilog;
using Serilog.Events; 
using Serilog.Templates;
using Serilog.Templates.Themes;
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
using HeadPats.Commands.Slash.Commission;
using HeadPats.Commands.Slash.Owner;
using HeadPats.Commands.Slash.UserLove;
using HeadPats.Commands.Slash.UserLove.Leaderboards;
using HeadPats.Events;
using HeadPats.Managers;
using HeadPats.Modules;
using HeadPats.Utils.ExternalApis;
using Serilog.Core;

namespace HeadPats;

public class Program {
    public static Program Instance { get; private set; }
    private static readonly ILogger Logger = Log.ForContext("SourceContext", Vars.Name);
    private static readonly ILogger UtilLogger = Log.ForContext("SourceContext", "Util");
    private static readonly ILogger CrLogger = Log.ForContext("SourceContext", "ClientReady");
    
    public DiscordSocketClient Client { get; set; }
    private InteractionService GlobalInteractions { get; set; }
    private InteractionService PersonalizedMembersInteractions { get; set; }
    private InteractionService BangerInteractions { get; set; }
    private InteractionService MintyLabsInteractions { get; set; }
    private CommandService Commands { get; set; }
    public FluxpointClient FluxpointClient { get; set; }
    public CookieClient CookieClient { get; set; }
    
    public SocketTextChannel? GeneralLogChannel { get; set; }
    public SocketTextChannel? ErrorLogChannel { get; set; }
    public SocketCategoryChannel? DmCategory { get; set; }
    
    private static List<EventModule> _eventModules = new();
    private static List<BasicModule> _basicModules = new();

    public Patreon_Client PatreonClientInstance;

    public static Task Main(string[] args) {
        Vars.IsWindows = Environment.OSVersion.ToString().Contains("windows", StringComparison.CurrentCultureIgnoreCase);
        Console.Title = $"{Vars.Name} v{Vars.Version} | Starting...";
        Logger.Debug($"{Vars.Name} Bot is starting . . .");
        new Program().MainAsync().GetAwaiter().GetResult();
        return Task.CompletedTask;
    }

    private Program() {
        Instance = this;
        Log.Logger = 
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(
                    initialMinimumLevel: Vars.IsDebug ? LogEventLevel.Debug : LogEventLevel.Information))
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
        if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Console.Title = "HeadPat.NET | Enter your bot token";
            Logger.Information("Please enter your bot token:");
            Config.Base.BotToken = Console.ReadLine()!.Trim();
        }
        else if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Logger.Information("Cannot proceed without a bot token. Please enter your bot token in the Configuration.json file.");
            Environment.Exit(0);
        }
        Config.Save();
        
        if (Vars.IsWindows)
            Console.Title = $"{Vars.Name} | Loading...";
        
        _basicModules.Add(new LoopingTaskScheduler());
        _basicModules.Add(new DNetToConsole());
        if (!Vars.IsDebug) 
            MobileManager.Initialize();
        
        Client = new DiscordSocketClient(new DiscordSocketConfig {
            // AlwaysDownloadUsers = true,
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            MessageCacheSize = 2000,
            GatewayIntents = GatewayIntents.Guilds
        });
        
        Commands = new CommandService(new CommandServiceConfig {
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = Discord.Commands.RunMode.Async,
            CaseSensitiveCommands = false,
            ThrowOnError = true
        });
        
        GlobalInteractions = new InteractionService(Client, new InteractionServiceConfig {
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = Discord.Interactions.RunMode.Async,
            ThrowOnError = true
        });
        
        PersonalizedMembersInteractions = new InteractionService(Client, new InteractionServiceConfig {
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            DefaultRunMode = Discord.Interactions.RunMode.Async,
            ThrowOnError = true
        });
        
        BangerInteractions = new InteractionService(Client, new InteractionServiceConfig {
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
            var severity = msg.Severity switch {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            Log.Write(severity, msg.Exception, "[{source}] {message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        };

        var argPos = 0;
        Client.MessageReceived += async arg => {
            // Don't process the command if it was a system message
            if (arg is not SocketUserMessage message)
                return;

            // Create a number to track where the prefix ends and the command begins

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!message.HasStringPrefix(Vars.IsDebug ? ".." : Config.Base.Prefix, ref argPos) || message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(Client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await Commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        };

        Client.Ready += ClientOnReady;
        
        var serviceCollection = new ServiceCollection();
        
        #region Slash Comands
        
        // Global
        await GlobalInteractions.AddModuleAsync<ContextMenuLove>(null);
        await GlobalInteractions.AddModuleAsync<Basic>(null);
        await GlobalInteractions.AddModuleAsync<ColorCmds>(null);
        // await GlobalInteractions.AddModuleAsync<IrlQuoteCmds>(null);
        // await GlobalInteractions.AddModuleAsync<Reply>(null);
        await GlobalInteractions.AddModuleAsync<TopCookie>(null);
        await GlobalInteractions.AddModuleAsync<TopPat>(null);
        await GlobalInteractions.AddModuleAsync<Summon>(null);
        await GlobalInteractions.AddModuleAsync<Love>(null);
        await GlobalInteractions.AddModuleAsync<DailyPatCmds>(null);
        
        // Commission
        await BangerInteractions.AddModuleAsync<Banger>(null);
        await PersonalizedMembersInteractions.AddModuleAsync<PersonalizedMembers>(null);
        
        // Owner
        await MintyLabsInteractions.AddModuleAsync<Contributors>(null);
        await MintyLabsInteractions.AddModuleAsync<BotControl>(null);
        await MintyLabsInteractions.AddModuleAsync<ConfigControl>(null);

        Client.InteractionCreated += async arg => {
            var iLogger = Log.ForContext("SourceContext", "Interaction");
            await BangerInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            await PersonalizedMembersInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            await GlobalInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            await MintyLabsInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            iLogger.Debug("{0} ran in guild {1}", arg.User.Username, arg.GuildId);
        };
        
        #endregion
        
        foreach (var module in _basicModules) {
            module.Initialize();
            module.InitializeClient(Client);
        }
        
        _eventModules.Add(new BangerEventListener());
        _eventModules.Add(new MessageReceived());
        _eventModules.Add(new OnBotJoinOrLeave());
        _eventModules.Add(new UserLeft());
        _eventModules.Add(new GuildUpdated());
        _eventModules.ForEach(module => module.Initialize(Client));
        
        if (!string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.CookieClientApiKey))
            CookieClient = new CookieClient(Config.Base.Api.ApiKeys.CookieClientApiKey!);
        
        if (!string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.FluxpointApiKey!))
            FluxpointClient = new FluxpointClient(Vars.Name, Config.Base.Api.ApiKeys.FluxpointApiKey!);

        if (string.IsNullOrWhiteSpace(Config.Base.Database.Database)) {
            Logger.Error("Cannot proceed without a database name. Exiting...");
            if (!Vars.IsWindows) {
                Utils.Execution.PM2.Stop();
            }
            Environment.Exit(0);
            return;
        }
        
        await using var db = new Context();
        var check = db.GlobalVariables.AsQueryable().FirstOrDefault(x => x.Name.Equals("PatCount") && x.ApplicationId.Equals(Vars.ClientId));
        if (check == null) {
            var global = new GlobalVariable {
                Name = "PatCount",
                ApplicationId = Vars.ClientId,
                Value = "0"
            };
            db.GlobalVariables.Add(global);
            await db.SaveChangesAsync();
        }
        
        Logger.Information("Bot finished initializing, logging in to Discord...");
        await Client.LoginAsync(TokenType.Bot, Config.Base.BotToken);
        await Client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task ClientOnReady() {
        // var globalCommands = await Client.GetGlobalApplicationCommandsAsync();
        // foreach (var cmds in globalCommands) {
        //     await cmds.DeleteAsync();
        //     Log.Debug("Deleted global slash command {0}", cmds.Name);
        // }
        
        Vars.StartTime = DateTime.UtcNow;
        CrLogger.Debug("Bot Version        = " + Vars.Version);
        CrLogger.Debug("Process ID         = " + Environment.ProcessId);
        CrLogger.Debug("Build Date         = " + Vars.BuildDate);
        CrLogger.Debug("Current OS         = " + (Vars.IsWindows ? "Windows" : "Linux"));
        CrLogger.Debug("Token              = " + Config.Base.BotToken!.Redact());
        // Log.Debug("Prefix (non-Slash) = " + $"{Config.Base.Prefix}");
        CrLogger.Debug("ActivityType       = " + $"{Config.Base.ActivityType}");
        CrLogger.Debug("Game               = " + $"{Config.Base.ActivityText}");
        CrLogger.Debug("Number of Commands = " + $"{GlobalInteractions.SlashCommands.Count + BangerInteractions.SlashCommands.Count + PersonalizedMembersInteractions.SlashCommands.Count + MintyLabsInteractions.SlashCommands.Count}");

        await Client.SetStatusAsync(Vars.IsDebug || Vars.IsWindows ? UserStatus.DoNotDisturb : UserStatus.Online);
        if (Vars.IsWindows)
            await Client.SetGameAsync(name: "Coding in Rider", type: ActivityType.CustomStatus);
        
        if (Vars.IsWindows) {
            var temp1 = Config.Base.ActivityText!.Equals("(insert game here)") || string.IsNullOrWhiteSpace(Config.Base.ActivityText!);
            Console.Title = $"{Vars.Name} v{Vars.Version} | Logged in as {Client.CurrentUser.Username} - " +
                            $"Currently in {Client.Guilds.Count} Guilds - " +
                            $"{Config.Base.ActivityType} {(temp1 ? "unset" : Config.Base.ActivityText)}";
        }
        
        OnBotJoinOrLeave.GuildIds = new List<ulong>();
        await using var db = new Context();
        var tempPatCount = Convert.ToInt32(db.GlobalVariables.AsQueryable().ToList().FirstOrDefault(x => x.Name.Equals("PatCount"))!.Value);
        
        foreach (var guild in Client.Guilds) {
            OnBotJoinOrLeave.GuildIds.Add(guild.Id);
        }

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
        CrLogger.Information("[{0}] Registered global slash commands.", "Register");
        try {
            await PersonalizedMembersInteractions.RegisterCommandsToGuildAsync(805663181170802719);
            CrLogger.Information("[{0}] Registered Personalized Member guild slash commands for {1}.", "Register", "805663181170802719");
        }
        catch (Exception e) {
            CrLogger.Error("Failed to register Personalized Member guild slash commands for {0}\n{err}", "805663181170802719", e);
        }

        if (!Vars.IsDebug) {
            try {
                await PersonalizedMembersInteractions.RegisterCommandsToGuildAsync(977705960544014407);
                CrLogger.Information("[{0}] Registered Personalized Member guild slash commands for {1}.", "Register", "977705960544014407");
            }
            catch (Exception e) {
                CrLogger.Error("Failed to register Personalized Member guild slash commands for {0}\n{err}", "977705960544014407", e);
            }

            try {
                await BangerInteractions.RegisterCommandsToGuildAsync(977705960544014407);
                CrLogger.Information("[{0}] Registered Banger guild slash commands for {1}.", "Register", "977705960544014407");
            }
            catch (Exception e) {
                CrLogger.Error("Failed to register Banger guild slash commands for {0}\n{err}", "977705960544014407", e);
            }
        }

        try {
            await MintyLabsInteractions.RegisterCommandsToGuildAsync(Vars.SupportServerId);
            CrLogger.Information("[{0}] Registered Owner slash commands for {1} ({2}).", "Register", "Minty Labs",  Vars.SupportServerId);
        }
        catch (Exception e) {
            CrLogger.Error("Failed to register Owner slash commands for guild {0}\n{err}", Vars.SupportServerId, e);
        }
        
        PatreonClientInstance = new Patreon_Client();
        await PatreonClientInstance.GetPatreonInfo();
        
        FindActiveGuilds.Start();
        
        await Task.Delay(TimeSpan.FromSeconds(5));
        OnBotJoinOrLeave.DoNotRunOnStart = false;
        if (Vars.IsWindows)
            await Client.SetGameAsync(name: "Coding in Rider", type: ActivityType.CustomStatus);
    }

    public SocketTextChannel? GetChannel(ulong guildId, ulong id) {
        var guild = Client.GetGuild(guildId);
        if (guild is null) {
            UtilLogger.Error($"Selected guild {guildId} does not exist!");
            return null;
        }
        if (guild.GetTextChannel(id) is { } channel) return channel;
        UtilLogger.Error($"Selected channel {id} does not exist!");
        return null;
    }
    
    public SocketUser? GetUser(ulong id) {
        if (Client.GetUser(id) is { } user) return user;
        UtilLogger.Error($"Selected user {id} does not exist!");
        return null;
    }
    
    public SocketGuild? GetGuild(ulong id) {
        if (Client.GetGuild(id) is { } guild) return guild;
        UtilLogger.Error($"Selected guild {id} does not exist!");
        return null;
    }
    
    public SocketUser? GetGuildUser(ulong guildId, ulong userId) {
        var guild = Client.GetGuild(guildId);
        if (guild is null) {
            UtilLogger.Error($"Selected guild {guildId} does not exist! <GetGuildUser>");
            return null;
        }
        if (guild.GetUser(userId) is { } user) return user;
        UtilLogger.Error($"Selected user {userId} does not exist! <GetGuildUser>");
        return null;
    }
    
    public SocketCategoryChannel? GetCategory(ulong guildId, ulong id) {
        var guild = Client.GetGuild(guildId);
        if (guild is null) {
            UtilLogger.Error($"Selected guild {guildId} does not exist!");
            return null;
        }
        if (guild.GetCategoryChannel(id) is { } category) return category;
        UtilLogger.Error($"Selected category {id} does not exist!");
        return null;
    }
}