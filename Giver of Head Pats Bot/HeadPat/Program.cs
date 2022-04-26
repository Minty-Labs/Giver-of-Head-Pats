using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using DSharpPlus.SlashCommands;
using HeadPats.Managers;
using HeadPats.Utils;
using NekosSharp;
using Pastel;

namespace HeadPats;

public static class BuildInfo {
    public const string Version = "4.0.0-002";
    public const string DSharpVer = "4.3.0-nightly-01123";
    public const string MintAPIVer = "1.4.0";
    public const string Name = "Giver of Head Pats";
    public const ulong ClientID = 821768206871167016;
#if DEBUG
    private static readonly DateTime ShortBuildDate = DateTime.Now;
#elif !DEBUG
    private static readonly DateTime ShortBuildDate = new(2022, 4, 25, 0, 0, 00); // (year, month, day, hour, min, sec)
#endif
    public static string BuildDateShort = $"{ShortBuildDate.Day} {GetMonth(ShortBuildDate.Month)} @ {ShortBuildDate.Hour}:{ChangeSingleNumber(ShortBuildDate.Minute)}";
    public static string BuildDate = $"Last Updated: {BuildDateShort}";
    public static DateTime StartTime = new();
    
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
        
    public static Config Config = Configuration._conf;
    public static Process? ThisProcess { get; set; }
}

public sealed class Program {
    public static DiscordClient? Client { get; set; }
    private CommandsNextExtension? Commands { get; set; }
    public static SlashCommandsExtension? Slash { get; set; }
    
    public static NekoClient? NekoClient { get; set; }
    
    private static void Main(string[] args) {
        Console.Title = string.Format($"{BuildInfo.Name} v{BuildInfo.Version}");
        new Program().MainAsync().GetAwaiter().GetResult();
    }
    
    private Program() {
        Logger.ConsoleLogger();
        Logger.Log("Elly is an adorable cute floof");
    }

    private async Task MainAsync() {
        Logger.Log("Bot is starting . . .");
        MobileManager.CreateMobilePatch();
        Client = new DiscordClient(new DiscordConfiguration {
            MessageCacheSize = 100,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.None,
            Token = BuildInfo.Config.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            AutoReconnect = true
        });
            
        Logger.ReplaceDSharpLogs(Client);

        var serviceCollection = new ServiceCollection();

        var commandsNextConfiguration = new CommandsNextConfiguration {
            StringPrefixes = new[] { BuildInfo.Config.Prefix.ToLower(), BuildInfo.Config.Prefix },
            EnableDefaultHelp = true
        };

        Commands = Client.UseCommandsNext(commandsNextConfiguration);
        Slash = Client.UseSlashCommands();


        Managers.Commands.Register(Commands);
        Commands.CommandExecuted += Commands_CommandExecuted;
        Commands.CommandErrored += Commands_CommandErrored;

        Managers.Commands.Register(Slash);
        Slash.SlashCommandErrored += Slash_SlashCommandErrored;

        Client.Ready += Client_Ready;
        var meh = new Handlers.EventHandler(Client); // Setup Command Handler
            
        await Client.ConnectAsync();

        await Task.Delay(-1);
    }

    private async Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e) {
        BuildInfo.StartTime = DateTime.Now;
        BuildInfo.ThisProcess = Process.GetCurrentProcess();
        Logger.Log("Welcome, " + "Lily".Pastel("9fffe3"));
        Logger.Log("Bot Version                   = " + BuildInfo.Version);
        Logger.Log("Process ID                    = " + BuildInfo.ThisProcess.Id);
        Logger.Log("Build Date                    = " + BuildInfo.BuildDateShort);
        Logger.WriteSeperator("C75450");
        Logger.Log("Token                         = " + OutputStringAsHidden(BuildInfo.Config.Token).Pastel("FBADBC"));
        Logger.Log("Prefix                        = " + $"{BuildInfo.Config.Prefix}".Pastel("FBADBC"));
        Logger.Log("ActivityType                  = " + $"{BuildInfo.Config.ActivityType}".Pastel("FBADBC"));
        Logger.Log("Game                          = " + $"{BuildInfo.Config.Game}".Pastel("FBADBC"));
        Logger.Log("Streaming URL                 = " + $"{BuildInfo.Config.StreamingUrl}".Pastel("FBADBC"));
        Logger.Log("Number of Commands            = " + $"{Commands?.RegisteredCommands.Count + Slash?.RegisteredCommands.Count}".Pastel("FBADBC"));
        //Logger.Log("Active Events                 = " + $"16".Pastel("FBADBC"));
        await Client!.UpdateStatusAsync(new DiscordActivity {
            Name = $"{BuildInfo.Config.Game}",
            ActivityType = _ActivityType(BuildInfo.Config.ActivityType)
        }, UserStatus.Online);
        Console.Title = string.Format($"{BuildInfo.Name} v{BuildInfo.Version} - {BuildInfo.Config.Game}");
    }

    private Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e) {
        Logger.CommandExecuted(e.Command.Name, e.Context.Message.Author.Username, e.Context.Guild.Name);
        return Task.CompletedTask;
    }

    private Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e) {
        if (e.Command == null)
            Logger.CommandNull(e.Context.Member!.Username, e.Context.Message.Content);
        else
            Logger.CommandErrored(e.Command.Name, e.Context.Message.Author.Username, e.Context.Guild.Name, e.Exception);
        return Task.CompletedTask;
    }

    private Task Slash_SlashCommandErrored(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs e) {
        Logger.CommandErrored(e.Context.CommandName, e.Context.User.Username, e.Context.Guild.Name, e.Exception, true);
        return Task.CompletedTask;
    }
    
    private static string OutputStringAsHidden(string s) {
        string? temp = null;
        for (var i = 0; i < s.Length; i++)
            temp += "*";
        return temp ?? "***************";
    }
    
    public static ActivityType _ActivityType(string type) {
        return type.ToLower() switch {
            "playing" => ActivityType.Playing,
            "listening" => ActivityType.ListeningTo,
            "watching" => ActivityType.Watching,
            "streaming" => ActivityType.Streaming,
            "play" => ActivityType.Playing,
            "listen" => ActivityType.ListeningTo,
            "watch" => ActivityType.Watching,
            "stream" => ActivityType.Streaming,
            _ => ActivityType.Custom,
        };
    }
}
