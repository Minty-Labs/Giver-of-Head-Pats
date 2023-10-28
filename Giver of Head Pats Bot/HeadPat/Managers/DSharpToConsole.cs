using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HeadPats.Modules;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers; 

public class DSharpToConsole : BasicModule {
    protected override string ModuleName => "DSharpToConsole";
    protected override string ModuleDescription => "Replaces DSharpPlus logs with Serilog";
    
    public override void InitializeClient(DiscordClient client) {
        client.ClientErrored += DiscordOnClientErrored;
        client.SocketErrored += OnSocketErrored;
        // client.Heartbeated += OnHeartbeated;
        client.SessionResumed += OnResumed;
        client.Zombied += OnZombied;
        client.SocketClosed += OnSocketClosed;
        // client.SocketOpened += OnSocketOpened;
        client.UnknownEvent += OnUnknownEvent;
        
        Log.Information("Replacing DSharpPlus logs as Serilog");
    }
    
    private static Task DiscordOnClientErrored(DiscordClient client, ClientErrorEventArgs args) {
        var message = $"[DSharp] [Client] Client error: {args.Exception}";
        Log.Error(message);
        if (Vars.IsDebug)
            SendErrorToLoggingChannel(message);
        return Task.CompletedTask;
    }
    
    private static Task OnSocketErrored(DiscordClient client, SocketErrorEventArgs args) {
        var message = $"[DSharp] [Socket] Socket error: {args.Exception}";
        Log.Error(message);
        return Task.CompletedTask;
    }
    
    private static Task OnResumed(DiscordClient client, SessionReadyEventArgs args) {
        if (Vars.IsDebug)
            Log.Information("[DSharp] Resumed Client");
        return Task.CompletedTask;
    }
    
    private static Task OnZombied(DiscordClient client, ZombiedEventArgs args) {
        Log.Information($"[DSharp] Client zombied: {args}");
        return Task.CompletedTask;
    }
    
    private static Task OnSocketClosed(DiscordClient client, SocketCloseEventArgs args) {
        if (args.CloseCode is 4000 or -1) {
            Log.Information($"[DSharp] [Socket] Socket closed: Code: {args.CloseCode} - Message: Connection terminated, attempting to reconnect...");
            return Task.CompletedTask;
        }
        Log.Information($"[DSharp] [Socket] Socket closed: Code: {args.CloseCode} - Message: {args.CloseMessage}");
        return Task.CompletedTask;
    }
    
    private static Task OnUnknownEvent(DiscordClient client, UnknownEventArgs args) {
        if (args.EventName is
            "GUILD_JOIN_REQUEST_DELETE" or
            "GUILD_JOIN_REQUEST_UPDATE" or
            "GUILD_JOIN_REQUEST_CREATE" or
            "GUILD_AUDIT_LOG_ENTRY_CREATE" or
            "GUILD_AUDIT_LOG_ENTRY_UPDATE" or
            "GUILD_AUDIT_LOG_ENTRY_DELETE") {
            // Console.WriteLine($"[DSharp] Unknown event, {args.EventName}, has ran. Not going in Serilog");
            // if (args.Json.Contains("communication_disabled_until")) {
            //     
            // }
            return Task.CompletedTask;
        }
        
        Log.Warning($"[DSharp] Unknown event: {args.EventName}");
        // if (Vars.IsDebug)
        //     Log.Debug($"JSON:\n{args.Json}");
        return Task.CompletedTask;
    }

    private static DiscordEmbed? ErrorEmbed(object message) {
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
        Log.Error("{0}", message);

        return new DiscordEmbedBuilder {
            Color = Colors.HexToColor("FF2525"),
            Description = $"```{finalMessage}```",
            Footer = new DiscordEmbedBuilder.EmbedFooter {
                Text = Vars.Version
            },
            Timestamp = DateTime.Now
        }.Build();
    }

    public static async Task SendErrorToLoggingChannelAsync(object message) => await Program.Client!.SendMessageAsync(Program.ErrorLogChannel!, ErrorEmbed(message)!);

    public static void SendErrorToLoggingChannel(object message) => Program.Client!.SendMessageAsync(Program.ErrorLogChannel!, ErrorEmbed(message)!).GetAwaiter().GetResult();
}