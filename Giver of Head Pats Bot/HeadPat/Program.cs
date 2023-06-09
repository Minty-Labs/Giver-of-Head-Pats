﻿using DSharpPlus;
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
using Serilog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace HeadPats;

public sealed class Program {
    public static DiscordClient? Client { get; set; }
    public static CommandsNextExtension? Commands { get; set; }
    public static SlashCommandsExtension? Slash { get; set; }
    public static FluxpointClient? FluxpointClient { get; set; }
    public static CookieClient? CookieClient { get; set; }
    
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
        Log.Information("Elly and Ahri are adorable cute floofs, I love them very very very much!~");
    }

    private async Task MainAsync() {
        if (Vars.IsWindows && string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Console.Title = $"{Vars.Name} | Enter your bot token";
            Log.Information("Please enter your bot token:");
            Config.Base.BotToken = Console.ReadLine()!.Trim();
        }
        else if (string.IsNullOrWhiteSpace(Config.Base.BotToken)) {
            Log.Error("Cannot proceed without a bot token. Please enter your bot token in the Configuration.json file.");
            await Log.CloseAndFlushAsync();
            Environment.Exit(0);
        }
        Config.Save();
        
        if (Vars.IsWindows)
            Console.Title = $"{Vars.Name} | Loading...";
        
        if (!Vars.IsDebug)
            MobileManager.CreateMobilePatch();

        Client = new DiscordClient(new DiscordConfiguration {
            MessageCacheSize = 100,
            MinimumLogLevel = Vars.IsDebug || Vars.IsWindows ? LogLevel.Debug : LogLevel.None,
            Token = Config.Base.BotToken,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            AutoReconnect = true
        });
        
        DSharpToConsole.ReplaceDSharpLogs(Client);

        var serviceCollection = new ServiceCollection();

        var commandsNextConfiguration = new CommandsNextConfiguration {
            StringPrefixes = Vars.IsDebug ? new [] { ".." } : new[] { Config.Base.Prefix.ToLower(), Config.Base.Prefix },
            EnableDefaultHelp = true
        };

        Commands = Client.UseCommandsNext(commandsNextConfiguration);
        Slash = Client.UseSlashCommands();
        
        Commands.SetHelpFormatter<HelpFormatter>();
        
        #region Legacy Commands
        
        LegacyCommandHandler.Register(Commands);
        Commands.CommandExecuted += (sender, args) => {
            Log.Information($"Command {args.Command.Name}, executed by {args.Context.User.Username}, " +
                            $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name}");
            return Task.CompletedTask;
        };
        Commands.CommandErrored += (sender, args) => {
            // if (args.Context.Message.Content.StartsWith('-'))
            //     return Task.CompletedTask;
            
            Log.Information($"Command {args.Command!.Name}, executed by {args.Context.User.Username}, " +
                            $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name} failed with\n{args.Exception.Message}");
            return Task.CompletedTask;
        };
        
        #endregion

        #region Slash Commands

        SlashCommandHandler.Register(Slash);
        ContextMenuHandler.Register(Slash);
        Slash.SlashCommandExecuted += (sender, args) => {
            Log.Information($"Slash Command {args.Context.CommandName}, executed by {args.Context.User.Username}, " +
                            $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name}");
            return Task.CompletedTask;
        };
        Slash.SlashCommandErrored += (sender, args) => {
            var message = $"Slash Command {args.Context.CommandName}, executed by {args.Context.User.Username}, " +
                          $"in #{args.Context.Channel.Name}, in the guild {args.Context.Guild.Name} failed with\n{args.Exception.Message}";
            Log.Information(message);
            DSharpToConsole.SendErrorToLoggingChannel(message);
            return Task.CompletedTask;
        };

        #endregion

        Client.Ready += Client_Ready;
        var eventHandler = new Handlers.EventHandler(Client); // Setup Command Handler
        
        if (!string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.CookieClientApiKey))
            CookieClient = new CookieClient(Config.Base.Api.ApiKeys.CookieClientApiKey!);
        
        if (!string.IsNullOrWhiteSpace(Config.Base.Api.ApiKeys.FluxpointApiKey!))
            FluxpointClient = new FluxpointClient(Vars.Name, Config.Base.Api.ApiKeys.FluxpointApiKey!);
        
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
        
        Config.CreateFile();
            
        await Client.ConnectAsync();

        await Task.Delay(-1);
    }
    
    internal static DiscordChannel? GeneralLogChannel, ErrorLogChannel;

    private static async Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e) {
        Vars.StartTime = DateTime.Now;
        Vars.ThisProcess = Process.GetCurrentProcess();
        Log.Debug("Bot Version                    = " + Vars.Version);
        Log.Debug("Process ID                     = " + Vars.ThisProcess.Id);
        Log.Debug("Build Date                     = " + Vars.BuildDate);
        Log.Debug("Current OS                     = " + (Vars.IsWindows ? "Windows" : "Linux"));
        Log.Debug("Token                          = " + Config.Base.BotToken!.Redact());
        Log.Debug("Prefix (non-Slash)             = " + $"{Config.Base.Prefix}");
        Log.Debug("ActivityType                   = " + $"{Config.Base.ActivityType}");
        Log.Debug("Game                           = " + $"{Config.Base.Activity}");
        Log.Debug("Streaming URL                  = " + $"{Config.Base.StreamingUrl}");
        Log.Debug("Number of Commands (non-Slash) = " + $"{Commands?.RegisteredCommands.Count + Slash?.RegisteredCommands.Count}");
        await Client!.UpdateStatusAsync(new DiscordActivity {
            Name = $"{Config.Base.Activity}",
            ActivityType = GetActivityType(Config.Base.ActivityType)
        }, Vars.IsDebug || Vars.IsWindows ? UserStatus.Idle : UserStatus.Online);

        if (Vars.IsWindows) {
            var temp1 = Config.Base.Activity!.Equals("(insert game here)") || string.IsNullOrWhiteSpace(Config.Base.Activity!);
            Console.Title = $"{Vars.Name} v{Vars.Version} | Logged in as {sender.CurrentUser.Username} - " +
                            $"Currently in {Client.Guilds.Count} Guilds - " +
                            $"{Config.Base.ActivityType} {(temp1 ? "unset" : Config.Base.Activity)}";
        }

        await using var db = new Context();
        var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;
        
        var startEmbed = new DiscordEmbedBuilder {
            Color = Vars.IsDebug || Vars.IsWindows ? DiscordColor.Yellow : DiscordColor.SpringGreen,
            Description = $"Bot has started on {(Vars.IsWindows ? "Windows" : "Linux")}\n" +
                          $"Currently in {sender.Guilds.Count} Guilds with {tempPatCount} total head pats given",
            Footer = new DiscordEmbedBuilder.EmbedFooter {
                Text = $"v{Vars.Version}",
                IconUrl = Client.CurrentUser.AvatarUrl
            },
            Timestamp = DateTime.Now
        }
            .AddField("Build Time", $"{Vars.BuildTime:F}\n<t:{Vars.BuildTime.GetSecondsFromUnixTime()}:R>")
            .AddField("Start Time", $"{DateTime.Now:F}\n<t:{DateTime.Now.GetSecondsFromUnixTime()}:R>")
            .AddField("DSharpPlus Version", Vars.DSharpVer)
            .Build();
        
        GeneralLogChannel =         await sender.GetChannelAsync(Config.Base.BotLogsChannel);
        ErrorLogChannel =           await sender.GetChannelAsync(Config.Base.ErrorLogsChannel);
        MessageCreated.DmCategory = await sender.GetChannelAsync(Config.Base.DmCategory);
        LoopingTaskScheduler.StartLoop();
        Handlers.Events.BangerEventListener.OnStartup();
        // await AutoRemoveOldDmChannels.RemoveOldDmChannelsTask();
        await sender.SendMessageAsync(GeneralLogChannel, startEmbed);
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
}
