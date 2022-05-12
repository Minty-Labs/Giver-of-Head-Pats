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
    public async Task ListTriggers(cc c, string start = "1", string end = "10") {
        var legend = new StringBuilder();
        legend.AppendLine("**Trigger**");
        legend.AppendLine("Response");
        legend.AppendLine("Respond only to the trigger alone");

        ReplyDic = GetResponseListPerPage(c, int.Parse(start), int.Parse(end));

        // var interactivity = c.Client.GetInteractivity();
        // var buttonPrev = new DiscordButtonComponent(ButtonStyle.Primary, "response_previous_page", null, 
        //     true, new DiscordComponentEmoji("⏪"));
        // var buttonNext = new DiscordButtonComponent(ButtonStyle.Primary, "response_next_page", null, 
        //     true, new DiscordComponentEmoji("⏩"));

        var sb = new StringBuilder();
        foreach (var r in ReplyDic) {
            sb.AppendLine(r.Value);
        }

        try {
            var e = new DiscordEmbedBuilder();
            e.WithTitle("Server's Auto Responses");
            e.AddField("Legend", legend.ToString());
            // e.AddField("Responses : Page #1 _(10 per page)_",sb.ToString());
            e.AddField("Responses _(first 10)_", sb.ToString());
            e.WithColor(Colors.HexToColor("58A1E0"));
            e.WithFooter(FooterText());
            e.WithTimestamp(DateTime.Now);
            await c.RespondAsync(e.Build());
        }
        catch (Exception eeeee) {
            await c.RespondAsync(
                "An error has occured listing the server's aut responses, I bet the character limit of 2000 was exceeded.");
            Logger.SendLog(eeeee);
        }

        // var options = new List<DiscordSelectComponentOption>() {
        //     new DiscordSelectComponentOption("Page 2", "page_2"),
        //     new DiscordSelectComponentOption("Page 1", "page_1", null, true)
        // };
        //
        // var drop = new DiscordSelectComponent("dropdown_page_select", null, options, false, 1, 2);
        //
        // var builder = new DiscordMessageBuilder();
        // builder.WithEmbed(e.Build());
        // builder.AddComponents(/*buttonPrev, buttonNext, */drop);
        // await builder.SendAsync(c.Message.Channel);

        // var m = await c.RespondAsync(builder);
        //
        // var result = await interactivity.WaitForButtonAsync(m, "response_next_page", CancellationToken.None);
        //
        // if (!result.TimedOut) await c.Client.SendMessageAsync(c.Message.Channel, "Yes");
        //
        // c.Client.ComponentInteractionCreated += async (s, args) => {
        //     
        //     //await m.ModifyAsync();
        // };
    }

    #region List Reply Dictionary Stuff

    private Dictionary<int, string> ReplyDic;

    private static Dictionary<int, string> GetResponseListPerPage(cc c, int startIndex, int endIndex) {
        var list = ReplyStructure.GetListOfReplies();
        var triggers = new StringBuilder();
        var dic = new Dictionary<int, string>();
        var num = 0;
        if (list == null) return new Dictionary<int, string>();
        
        foreach (var t in list) {
            //if (num.IsInRange(1, 15)) break;
            var r = ReplyStructure.GetResponse(t.Trigger, c.Guild.Id);
            var i = ReplyStructure.GetInfo(t.Trigger, c.Guild.Id);

            triggers.AppendLine($"**{t.Trigger}**");
            triggers.AppendLine(r);
            triggers.AppendLine(i);
            triggers.AppendLine();

            //num++;
            dic.Add(num++, triggers.ToString());
        }

        dic = dic.OrderBy(d => d.Key).Skip(startIndex).Take(endIndex - startIndex + 1)
            .ToDictionary(k => k.Key, v => v.Value);

        return dic;
    }

    #endregion
    
}