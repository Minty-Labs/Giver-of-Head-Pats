using Discord.WebSocket;
using HeadPats.Modules;
using HeadPats.Utils;
using Discord;
using HeadPats.Configuration;
using HeadPats.Events;
using Serilog;

namespace HeadPats.Managers; 

public class DNetToConsole : BasicModule {
    protected override string ModuleName => "D.NET To Console";
    protected override string ModuleDescription => "Replaces D.NET logs with Serilog";
    private static readonly ILogger Logger = Log.ForContext(typeof(DNetToConsole));
    private static readonly ILogger DnLogger = Log.ForContext("SourceContext", "DNET");
    
    public override void InitializeClient(DiscordSocketClient client) {
        client.Connected += OnResumed;
        client.Disconnected += OnSocketClosed;
        
        Logger.Information("Replacing D.NETPlus logs as Serilog");
    }
    
    private static Task OnResumed() {
        if (Vars.IsDebug)
            Logger.Information("Resumed Client");
        return Task.CompletedTask;
    }
    
    private static Task OnSocketClosed(Exception exception) {
        DnLogger.Information($"[Socket] Socket disconnected: Source: {exception.Source} - Message: {exception.Message}");
        return Task.CompletedTask;
    }

    private static EmbedBuilder? ErrorEmbed(object message, object? exception = null) {
        var messageToString = message.ToString();
        var finalMessage = (messageToString!.Length > 2000 ? messageToString[..1990] + "..." : messageToString) ?? "Error, no message could be displayed. This should not happen.";
        // var forceSendNormalMessage = false;
        
        if ((finalMessage.Contains("Unauthorized: 403") && finalMessage.Contains("DiscordApiClient")) || 
            (finalMessage.Contains("Slash Command dailypat,") && finalMessage.Contains("SlashCommandsExtension.RunPreexecutionChecksAsync")) ||
            (finalMessage.Contains("banger") && finalMessage.Contains("BadRequest") && finalMessage.Contains("ExecuteRequestAsync") && finalMessage.Contains("TRequest")) ||
            (finalMessage.Contains("personalization") && finalMessage.Contains("BadRequest") && finalMessage.Contains("ExecuteRequestAsync") && finalMessage.Contains("TRequest")))
            return null; // Break if contains
        // if (finalMessage.Contains("Bad request: 400") && finalMessage.Contains("CreateWebhookAsync"))
        //     forceSendNormalMessage = true;
        Logger.Error("{0}", message);

        return new EmbedBuilder {
            Color = Color.Red,
            Description = exception != null ? $"{MarkdownUtils.ToCodeBlockMultiline(finalMessage)}\n{MarkdownUtils.ToCodeBlockMultiline(exception.ToString() ?? "empty exception")}" : MarkdownUtils.ToCodeBlockMultiline(finalMessage),
            Footer = new EmbedFooterBuilder {
                Text = Vars.VersionStr
            },
            Timestamp = DateTime.Now
        };
    }

    private static EmbedBuilder? LoggingEmbed(object message, object? line2 = null, object? line3 = null) {
        var messageToString = message.ToString();
        var finalMessage = (messageToString!.Length > 2000 ? messageToString[..1990] + "..." : messageToString) ?? "Error, no message could be displayed. This should not happen.";
        Logger.Information("{0}", message);
        return new EmbedBuilder {
            Color = Color.Green,
            Description = messageToString + (line2 != null ? $"\n{line2}" : "") + (line3 != null ? $"\n{line3}" : ""),
            Footer = new EmbedFooterBuilder {
                Text = Vars.VersionStr
            },
            Timestamp = DateTime.Now
        };
    }

    public static async Task SendErrorToLoggingChannelAsync(object message, MessageReference? reference = null) => await Program.Instance.ErrorLogChannel!.SendMessageAsync(embed: ErrorEmbed(message)!.Build(), messageReference: reference);

    public static void SendErrorToLoggingChannel(object message, MessageReference? reference = null) => SendErrorToLoggingChannelAsync(message, reference).GetAwaiter().GetResult();

    public static async Task SendErrorToLoggingChannelAsync(object message, MessageReference? reference = null, object? obj = null) {
        if (OnBotJoinOrLeave.DoNotRunOnStart)
            return;
        await Program.Instance.ErrorLogChannel!.SendMessageAsync(embed: ErrorEmbed(message, obj)!.Build(), messageReference: reference);
    }
    
    public static void SendErrorToLoggingChannel(object message, MessageReference? reference = null, object? obj = null) => SendErrorToLoggingChannelAsync(message, reference, obj).GetAwaiter().GetResult();
    
    public static async Task SendMessageToLoggingChannelAsync(object line1, object? line2 = null, object? line3 = null) => await Program.Instance.ErrorLogChannel!.SendMessageAsync(embed: LoggingEmbed(line1, line2, line3)!.Build());
}