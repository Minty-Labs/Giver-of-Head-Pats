using Discord.WebSocket;
using HeadPats.Modules;
using HeadPats.Utils;
using Discord;
using HeadPats.Configuration;
using Serilog;

namespace HeadPats.Managers; 

public class DNetToConsole : BasicModule {
    protected override string ModuleName => "D.NET To Console";
    protected override string ModuleDescription => "Replaces D.NET logs with Serilog";
    
    private static readonly ILogger Logger = Log.ForContext(typeof(DNetToConsole));
    
    public override void InitializeClient(DiscordSocketClient client) {
        client.Connected += OnResumed;
        client.Disconnected += OnSocketClosed;
        
        Logger.Information("Replacing D.NETPlus logs as Serilog");
    }
    
    private static Task OnResumed() {
        if (Vars.IsDebug)
            Logger.Information("[D.NET] Resumed Client");
        return Task.CompletedTask;
    }
    
    private static Task OnSocketClosed(Exception exception) {
        Logger.Information($"[D.NET] [Socket] Socket disconnected: Source: {exception.Source} - Message: {exception.Message}");
        return Task.CompletedTask;
    }

    private static Embed? ErrorEmbed(object message) {
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
            Color = Colors.HexToColor("FF2525"),
            Description = $"```{finalMessage}```",
            Footer = new EmbedFooterBuilder {
                Text = Vars.Version
            },
            Timestamp = DateTime.Now
        }.Build();
    }

    private static Embed? MessageEmbed(object message) 
        => new EmbedBuilder {
            Color = Colors.HexToColor("23A559"),
            Description = message.ToString(),
            Footer = new EmbedFooterBuilder {
                Text = Vars.Version
            },
            Timestamp = DateTime.Now
        }.Build();

    public static async Task SendErrorToLoggingChannelAsync(object message) => await Program.Instance.GetChannel(Vars.SupportServerId, Config.Base.ErrorLogsChannel)!.SendMessageAsync(embed: ErrorEmbed(message));

    public static void SendErrorToLoggingChannel(object message) => Program.Instance.GetChannel(Vars.SupportServerId, Config.Base.ErrorLogsChannel)!.SendMessageAsync(embed: ErrorEmbed(message)).GetAwaiter().GetResult();
    
    public static async Task SendMessageToLoggingChannelAsync(object message) => await Program.Instance.GetChannel(Vars.SupportServerId, Config.Base.BotLogsChannel)!.SendMessageAsync(embed: MessageEmbed(message));
}