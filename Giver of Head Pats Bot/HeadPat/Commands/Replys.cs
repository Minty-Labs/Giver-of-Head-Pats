using System.Globalization;
using System.IO.Compression;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Emzi0767;
using HeadPats.Data;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Replies : BaseCommandModule {
    public Replies() => Logger.Loadodule("Replies");
    
    private string FooterText(string extra = "")
        => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";

    [Command("AddReply"), Aliases("ar"), Description("Adds an auto response for the server")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task AddReply(cc c, string trigger = "", string response = "", string requireOnlyTriggerText = "false") {
        if (string.IsNullOrWhiteSpace(trigger) || string.IsNullOrWhiteSpace(response)) {
            await c.RespondAsync("Incorrect format. Please use the following format: " +
                                 $"`{BuildInfo.Config.Prefix}AddReply [\"Trigger\"] [\"Response\"] (\"RequireOnlyTriggerText\")`\n" +
                                 "_**Note:** Use \"Quotations\" to separate the trigger from the response._");
            return;
        }

        var b = requireOnlyTriggerText.ToLower() switch {
            "true" => true,
            "t" => true,
            _ => false
        };
        
        ReplyStructure.AddValue(c.Guild.Id, trigger, response, b);
        await c.RespondAsync("Saved trigger!");
    }

    [Command("RemoveReply"), Aliases("rr"), Description("Removes a trigger response by the provided trigger")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task RemoveReply(cc c, [RemainingText]string trigger) {
        ReplyStructure.RemoveValue(c.Guild.Id, trigger);
        if (ReplyStructure.ErroredOnRemove) {
            await c.RespondAsync("Either the provided trigger does not exist, or an error has occured.");
            if (BuildInfo.IsDebug)
                await c.Client.SendMessageAsync(c.Message.Channel, $"[Debug] Error: {ReplyStructure.ErroredException}");
        }
        else await c.RespondAsync($"Removed the trigger: {trigger}");
        ReplyStructure.ErroredOnRemove = false;
    }

    [Command("ListTriggers"), Aliases("lr", "listreplies", "listreplys"), Description("Lists the triggers for auto responses")]
    public async Task ListTriggers(cc c) {
        var legend = new StringBuilder();
        var list = ReplyStructure.GetListOfReplies();
        var triggers = new StringBuilder();
        legend.AppendLine("Trigger");
        legend.AppendLine("Response");
        legend.AppendLine("Respond only to the trigger alone");
        legend.AppendLine("==============================================================================");

        if (list != null) {
            foreach (var t in list) {
                var r = ReplyStructure.GetResponse(t.Trigger, c.Guild.Id);
                var i = ReplyStructure.GetInfo(t.Trigger, c.Guild.Id);

                triggers.AppendLine(t.Trigger);
                triggers.AppendLine(r);
                triggers.AppendLine(i);
                triggers.AppendLine();
            }
        }

        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        await sw.WriteLineAsync($"Triggers for {c.Guild.Name} ({c.Guild.Id})");
        await sw.WriteLineAsync();
        await sw.WriteLineAsync("-=- Legend -=-");
        await sw.WriteLineAsync(legend.ToString());
        await sw.WriteLineAsync();
        await sw.WriteLineAsync(triggers.ToString());
        
        await sw.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordMessageBuilder();
        builder.WithFile("Responses.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }
}