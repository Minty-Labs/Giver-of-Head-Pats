using Discord;
using Michiru.Configuration;

namespace Michiru.Managers; 

public static class ErrorSending {
    private static EmbedBuilder? ErrorEmbed(object message) {
        var msg = message.ToString();
        var finalMsg = (msg!.Length > 2000 ? msg[..1990] + "..." : msg) ?? "Error, no message could be displayed. This should not happen.";
        if (finalMsg.ToLower().Contains("unauthorized") && finalMsg.Contains("403"))
            return null;

        return new EmbedBuilder {
            Color = Color.Red,
            Description = $"```{finalMsg}```",
            Footer = new EmbedFooterBuilder {
                Text = Vars.Version
            },
            Timestamp = DateTime.Now
        };
    }

    public static async Task SendErrorToLoggingChannelAsync(object message, MessageReference? reference = null) => await Program.Instance.ErrorLogChannel.SendMessageAsync(embed: ErrorEmbed(message)!.Build(), messageReference: reference);

    public static void SendErrorToLoggingChannel(object message, MessageReference? reference = null) => SendErrorToLoggingChannelAsync(message, reference).GetAwaiter().GetResult();
}