using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Michiru.Commands;
using Michiru.Configuration;
using Michiru.Managers;
using Michiru.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using static System.DateTime;

namespace Michiru;

public class Program {
    public static Program Instance { get; private set; }
    private static readonly ILogger Logger = Log.ForContext("SourceContext", "Michiru");
    private static readonly ILogger UtilLogger = Log.ForContext("SourceContext", "Util");
    private static readonly ILogger BangerLogger = Log.ForContext("SourceContext", "Banger");
    public DiscordSocketClient Client { get; set; }
    private CommandService Commands { get; set; }
    private InteractionService GlobalInteractions { get; set; }
    public SocketTextChannel? GeneralLogChannel { get; set; }
    public SocketTextChannel? ErrorLogChannel { get; set; }

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
            Console.Title = $"{Vars.Name} | Enter your bot token";
            Console.Write("Please enter your bot token: ");
            Config.Base.BotToken = Console.ReadLine()!.Trim();
            
            if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
                Logger.Warning("Cannot proceed without a bot token. Please enter your bot token in the Michiru.Bot.config.json file.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        if (Config.Base.BotLogsChannel.IsZero()) 
            Logger.Warning("Bot Logs Channel is not set. Please set the BotLogsChannel in the Michiru.Bot.config.json file.");
        if (Config.Base.ErrorLogsChannel.IsZero()) 
            Logger.Warning("Error Logs Channel is not set. Please set the ErrorLogsChannel in the Michiru.Bot.config.json file.");
        Config.Save();

        if (Vars.IsWindows)
            Console.Title = $"{Vars.Name} | Loading...";
        if (!Vars.IsDebug)
            MobileManager.Initialize();

        Client = new DiscordSocketClient(new DiscordSocketConfig {
            AlwaysDownloadUsers = true,
            LogLevel = Vars.IsWindows ? LogSeverity.Verbose : LogSeverity.Debug, //Info,
            MessageCacheSize = 2000,
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildEmojis | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
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

        var argPos = 0;
        Client.MessageReceived += async arg => {
            // Don't process the command if it was a system message
            if (arg is not SocketUserMessage message)
                return;

            // Create a number to track where the prefix ends and the command begins

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!message.HasStringPrefix("-", ref argPos) || message.Author.IsBot)
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
        Client.MessageReceived += BangerListener;
        Client.GuildUpdated += OnGuildUpdated;

        var serviceCollection = new ServiceCollection();

        await Commands.AddModuleAsync<BasicCommandsThatIDoNotWantAsSlashCommands>(null);
        await GlobalInteractions.AddModuleAsync<Banger>(null);
        await GlobalInteractions.AddModuleAsync<Personalization>(null);

        Client.InteractionCreated += async arg => {
            var iLogger = Log.ForContext("SourceContext", "Interaction");
            await GlobalInteractions.ExecuteCommandAsync(new SocketInteractionContext(Client, arg), null);
            iLogger.Debug("{0} ran a command in guild {1}", arg.User.Username, arg.GuildId);
        };
        
        await Scheduler.Initialize();

        Logger.Information("Bot finished initializing, logging in to Discord...");
        await Client.LoginAsync(TokenType.Bot, Config.Base.BotToken);
        await Client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task ClientOnReady() {
        var crLogger = Log.ForContext("SourceContext", "ClientReady");
        Vars.StartTime = UtcNow;
        crLogger.Information("Bot Version        = " + Vars.Version);
        crLogger.Information("Process ID         = " + Environment.ProcessId);
        crLogger.Information("Build Date         = " + Vars.BuildDate);
        crLogger.Information("Current OS         = " + (Vars.IsWindows ? "Windows" : "Linux"));
        crLogger.Information("Token              = " + Config.Base.BotToken!.Redact());
        crLogger.Information("ActivityType       = " + $"{Config.Base.ActivityType}");
        crLogger.Information("Game               = " + $"{Config.Base.ActivityText}");
        crLogger.Information("Number of Commands = " + $"{GlobalInteractions.SlashCommands.Count + Commands.Commands.Count()}");

        if (Vars.IsWindows) {
            var temp1 = Config.Base.ActivityText!.Equals("(insert game here)") || string.IsNullOrWhiteSpace(Config.Base.ActivityText!);
            Console.Title = $"{Vars.Name} v{Vars.Version} | Logged in as {Client.CurrentUser.Username} - " +
                            $"Currently in {Client.Guilds.Count} Guilds - {Config.GetBangerNumber()} bangers posted -" +
                            $"{Config.Base.ActivityType} {(temp1 ? "unset" : Config.Base.ActivityText)}";
        }

        var startEmbed = new EmbedBuilder {
                Color = Vars.IsDebug || Vars.IsWindows ? Colors.HexToColor("5178b5") : Colors.MichiruPink,
                Description = $"Bot has started on {(Vars.IsWindows ? "Windows" : "Linux")}\n" +
                              $"Currently in {Client.Guilds.Count} Guilds\n" +
                              $"Currently listening to {Config.GetBangerNumber()} bangers",
                Footer = new EmbedFooterBuilder {
                    Text = $"v{Vars.Version}",
                    IconUrl = Client.CurrentUser.GetAvatarUrl()
                },
                Timestamp = Now
            }
            .AddField("Build Time", $"<t:{Vars.BuildTime.ToUniversalTime().GetSecondsFromUtcUnixTime()}:F>\n<t:{Vars.BuildTime.ToUniversalTime().GetSecondsFromUtcUnixTime()}:R>")
            .AddField("Start Time", $"<t:{UtcNow.GetSecondsFromUtcUnixTime()}:F>\n<t:{UtcNow.GetSecondsFromUtcUnixTime()}:R>")
            .AddField("Discord.NET Version", Vars.DNetVer)
            .Build();

        if (!Config.Base.ErrorLogsChannel.IsZero())
            ErrorLogChannel = GetChannel(Vars.SupportServerId, Config.Base.ErrorLogsChannel);
        if (!Config.Base.BotLogsChannel.IsZero()) {
            GeneralLogChannel = GetChannel(Vars.SupportServerId, Config.Base.BotLogsChannel);
            await GeneralLogChannel!.SendMessageAsync(embed: startEmbed);
        }

        await GlobalInteractions.RegisterCommandsGloballyAsync();
        crLogger.Information("Registered global slash commands.");
    }


    private static bool IsUrlWhitelisted(string url, ICollection<string> list) {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        return list?.Contains(uri.Host) ?? throw new ArgumentNullException(nameof(list));
    }

    private static bool IsFileExtWhitelisted(string extension, ICollection<string> list)
        => list?.Contains(extension) ?? throw new ArgumentNullException(nameof(list));

    private static Task BangerListener(SocketMessage args) {
        var socketUserMessage = (SocketUserMessage)args;
        var conf = Config.Base.Banger.FirstOrDefault(x => x.ChannelId == args.Channel.Id);
        if (conf is null) {
            // BangerLogger.Error("Banger config for guild does not exist!");
            // await ErrorSending.SendErrorToLoggingChannelAsync("Banger config for guild does not exist! Guild: " + GetGuildFromChannel(args.Channel.Id)!.Name);
            return Task.CompletedTask;
        }

        if (!conf!.Enabled) return Task.CompletedTask;
        if (args.Author.IsBot) return Task.CompletedTask;

        var messageContent = args.Content;
        if (messageContent.StartsWith('.')) return Task.CompletedTask; // can technically be exploited but whatever
        var attachments = args.Attachments;
        var stickers = args.Stickers;
        var upVote = conf.CustomUpvoteEmojiId != 0 ? EmojiUtils.GetCustomEmoji(conf.CustomUpvoteEmojiName, conf.CustomUpvoteEmojiId) : Emote.Parse(conf.CustomUpvoteEmojiName) ?? Emote.Parse(":thumbsup:");
        var downVote = conf.CustomDownvoteEmojiId != 0 ? EmojiUtils.GetCustomEmoji(conf.CustomDownvoteEmojiName, conf.CustomDownvoteEmojiId) : Emote.Parse(conf.CustomDownvoteEmojiName) ?? Emote.Parse(":thumbsdown:");

        var urlGood = IsUrlWhitelisted(messageContent, conf.WhitelistedUrls!);
        if (urlGood) {
            if (conf is { AddUpvoteEmoji: true, UseCustomUpvoteEmoji: true })
                socketUserMessage.AddReactionAsync(upVote);
            if (conf is { AddDownvoteEmoji: true, UseCustomDownvoteEmoji: true })
                socketUserMessage.AddReactionAsync(downVote);
            conf.SubmittedBangers++;
            Config.Save();
            return Task.CompletedTask;
        }

        if (conf.SpeakFreely) return Task.CompletedTask;
        BangerLogger.Information("Sent Bad URL Response");
        args.Channel.SendMessageAsync(conf.UrlErrorResponseMessage).DeleteAfter(5);
        args.DeleteAsync();

        if (!string.IsNullOrEmpty(messageContent) || (attachments.Count == 0 && stickers.Count == 0)) return Task.CompletedTask;
        var extGood = IsFileExtWhitelisted(attachments.First().Filename.Split('.').Last(), conf.WhitelistedFileExtensions!);
        if (extGood || (urlGood && extGood)) {
            if (conf is { AddUpvoteEmoji: true, UseCustomUpvoteEmoji: true })
                socketUserMessage.AddReactionAsync(upVote);
            if (conf is { AddDownvoteEmoji: true, UseCustomDownvoteEmoji: true })
                socketUserMessage.AddReactionAsync(downVote);
            conf.SubmittedBangers++;
            Config.Save();
            return Task.CompletedTask;
        }

        if (conf.SpeakFreely) return Task.CompletedTask;
        BangerLogger.Information("Sent Bad File Extension Response");
        args.Channel.SendMessageAsync(conf.FileErrorResponseMessage).DeleteAfter(5);
        args.DeleteAsync();

        return Task.CompletedTask;
    }
    
    private ulong _pennysGuildWatcherChannelId = 0;
    private ulong _pennysGuildWatcherGuildId = 0;
    private Task OnGuildUpdated(SocketGuild arg1, SocketGuild arg2) {
        if (arg1.Name == arg2.Name) return Task.CompletedTask;
        
        if (_pennysGuildWatcherGuildId == 0) _pennysGuildWatcherGuildId = Config.Base.PennysGuildWatcher.GuildId;
        if (arg1.Id != _pennysGuildWatcherGuildId) return Task.CompletedTask;
        
        if (_pennysGuildWatcherChannelId == 0) _pennysGuildWatcherChannelId = Config.Base.PennysGuildWatcher.ChannelId;
        var channel = arg1.GetTextChannel(_pennysGuildWatcherChannelId);
        if (channel is null) return Task.CompletedTask;
        
        var daysNumber = UtcNow.Subtract(Config.Base.PennysGuildWatcher.LastUpdateTime.UnixTimeStampToDateTime()).Days;
        var embed = new EmbedBuilder {
            Title = "Guild Name Updated",
            Description = $"It has been {(daysNumber < 1 ? "less than a day" : (daysNumber == 1 ? "1 day" : $"{daysNumber} days"))} since the last time the guild name was updated.",
            Color = Colors.HexToColor("0091FF")
        }
            .AddField("Old Name", arg1.Name)
            .AddField("New Name", arg2.Name);
        channel.SendMessageAsync(embed: embed.Build());
        Config.Base.PennysGuildWatcher.LastUpdateTime = UtcNow.ToUniversalTime().GetSecondsFromUtcUnixTime();
        Config.Save();
        return Task.CompletedTask;
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

    public SocketGuild? GetGuildFromChannel(ulong channelId) {
        var channel = Client.GetChannel(channelId);
        if (channel is null) {
            UtilLogger.Error("Selected channel {channelId} does not exist!", channelId);
            return null;
        }

        if (channel is SocketGuildChannel guildChannel) return guildChannel.Guild;
        UtilLogger.Error("Selected channel {channelId} is not a guild channel!", channelId);
        return null;
    }
}