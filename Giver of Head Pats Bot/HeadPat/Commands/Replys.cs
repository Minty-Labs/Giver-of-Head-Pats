using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands;

public class ReplyApplication : ApplicationCommandModule {

    [SlashCommandGroup("Reply", "Self-creating simple trigger-based command message outputs")]
    public class Replies : ApplicationCommandModule {
        [SlashCommand("add", "Adds an auto response for the server")]
        [SlashRequireUserPermissions(Permissions.ManageMessages)]
        public async Task AddReply(ic c,
            [Option("Trigger", "Word or phrase as the trigger", true)] string trigger,
            [Option("Response", "Response from trigger (add '<br>' for new lines)", true)] string response,
            [Choice("false", "false")] [Choice("true", "true")]
            [Option("RequireOnlyTriggerText", "Respond ONLY if the message is equal to the trigger?")] string requireOnlyTriggerText = "false",
            [Choice("false", "false")] [Choice("true", "true")]
            [Option("DeleteTrigger", "Auto Remove the trigger text message?")] string deleteTrigger = "false") {
        
            ReplyStructure.AddValue(c.Guild.Id, trigger, response, 
                StringUtils.GetBooleanFromString(requireOnlyTriggerText), StringUtils.GetBooleanFromString(deleteTrigger));
            await c.CreateResponseAsync("Trigger saved!");
        }
    
        [SlashCommand("Remove", "Removes a trigger response by the provided trigger", false)]
        [SlashRequireUserPermissions(Permissions.ManageMessages)]
        public async Task RemoveReply(ic c,
            [Option("Trigger", "Enter the trigger word or phrase exactly", true)]
            string trigger) {
            ReplyStructure.RemoveValue(c.Guild.Id, trigger);
            if (ReplyStructure.ErroredOnRemove) {
                await c.CreateResponseAsync("Either the provided trigger does not exist, or an error has occured.", true);
                if (BuildInfo.IsDebug)
                    await c.CreateResponseAsync($"[Debug] Error: {ReplyStructure.ErroredException}", true);
            }
            else await c.CreateResponseAsync($"Removed the trigger: {trigger}");
            ReplyStructure.ErroredOnRemove = false;
        }

        [SlashCommand("List", "Lists the triggers for auto responses")]
        public async Task ListTriggers(ic c) {
            var legend = new StringBuilder();
            var list = ReplyStructure.GetListOfReplies();
            var triggers = new StringBuilder();
            legend.AppendLine("Trigger");
            legend.AppendLine("Response");
            legend.AppendLine("Respond only to the trigger alone");
            legend.AppendLine("Does trigger message auto delete");
            legend.AppendLine("==============================================================================");

            if (list != null) {
                foreach (var t in list) {
                    if (t.GuildId != c.Guild.Id) continue;
                    var r = ReplyStructure.GetResponse(t.Trigger, c.Guild.Id);
                    var i = ReplyStructure.GetInfo(t.Trigger, c.Guild.Id);
                    var d = ReplyStructure.GetsDeleted(t.Trigger, c.Guild.Id);

                    triggers.AppendLine(t.Trigger);
                    triggers.AppendLine(r);
                    triggers.AppendLine(i);
                    triggers.AppendLine(d);
                    triggers.AppendLine();
                }
            }

            var triggerStr = triggers.ToString();

            if (string.IsNullOrWhiteSpace(triggerStr) || triggerStr.Equals("\n\n\n\n\n"))
                triggerStr = "Guild has no triggers for auto replies.";

            using var ms = new MemoryStream();
            await using var sw = new StreamWriter(ms);
            await sw.WriteLineAsync($"Triggers for {c.Guild.Name} ({c.Guild.Id})");
            await sw.WriteLineAsync();
            await sw.WriteLineAsync("-=- Legend -=-");
            await sw.WriteLineAsync(legend.ToString());
            await sw.WriteLineAsync();
            await sw.WriteLineAsync(triggerStr);

            await sw.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);
        
            var builder = new DiscordMessageBuilder();
            builder.AddFile($"{c.Guild.Name} Responses.txt", ms);
            // await c.CreateResponseAsync(new DiscordInteractionResponseBuilder(builder));
            await c.CreateResponseAsync("File Sent below", true);
            await Program.Client!.SendMessageAsync(c.Channel, builder);
        }
    }
}

/*public class Replies : BaseCommandModule {
    public Replies() => Logger.Loadodule("Replies");

    private void FooterText(DiscordEmbedBuilder em, string extraText = "") {
        em.WithTimestamp(DateTime.Now);
        em.WithFooter($"{(string.IsNullOrWhiteSpace(extraText) ? "" : $"{extraText}")}");
    }

    [Command("AddReply"), Aliases("ar"), Description("Adds an auto response for the server")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task AddReply(cc c, [Description("Trigger word or phrase (separated by \"Quotations\")")] string trigger = "",
        [Description("The response given that is triggered by the word or phrase")] string response = "",
        [Description("Boolean as string; Respond ONLY with the trigger word or phrase")] string requireOnlyTriggerText = "false") {
        if (string.IsNullOrWhiteSpace(trigger) || string.IsNullOrWhiteSpace(response)) {
            await c.RespondAsync("Incorrect format. Please use the following format: " +
                                 $"`{BuildInfo.Config.Prefix}AddReply [\"Trigger\"] [\"Response\"] (\"RequireOnlyTriggerText\")`\n" +
                                 "_**Note:** Use \"Quotations\" to separate the trigger from the response._");
            return;
        }

        var b = requireOnlyTriggerText.ToLower() switch {
            "true" => true,
            "t" => true,
            "yes" => true,
            "y" => true,
            _ => false
        };
        
        ReplyStructure.AddValue(c.Guild.Id, trigger, response, b);
        await c.RespondAsync("Saved trigger!");
    }

    [Command("RemoveReply"), Aliases("rr"), Description("Removes a trigger response by the provided trigger")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task RemoveReply(cc c, [RemainingText, Description("Trigger word or phrase (separated by \"Quotations\") to remove")] string trigger) {
        ReplyStructure.RemoveValue(c.Guild.Id, trigger);
        if (ReplyStructure.ErroredOnRemove) {
            await c.RespondAsync("Either the provided trigger does not exist, or an error has occured.");
            if (BuildInfo.IsDebug)
                await c.Client.SendMessageAsync(c.Message.Channel, $"[Debug] Error: {ReplyStructure.ErroredException}");
        }
        else await c.RespondAsync($"Removed the trigger: {trigger}");
        ReplyStructure.ErroredOnRemove = false;
    }

    [Command("ListTriggers"), Aliases("lr", "lt", "listreplies", "listreplys"), Description("Lists the triggers for auto responses")]
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
        builder.AddFile("Responses.txt", ms);
        await builder.WithReply(c.Message.Id).SendAsync(c.Channel);
    }
}*/