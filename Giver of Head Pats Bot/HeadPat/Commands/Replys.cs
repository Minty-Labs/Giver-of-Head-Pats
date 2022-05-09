using System.IO.Compression;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HeadPats.Data;
using HeadPats.Utils;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Replys : BaseCommandModule {
    public Replys() => Logger.Loadodule("Replys");
    
    private string FooterText(string extra = "")
        => $"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}{(string.IsNullOrWhiteSpace(extra) ? "" : $" • {extra}")}";

    [Command("AddReply"), Aliases("ar"), Description("Adds an auto response for the server")]
    [RequirePermissions(Permissions.ManageMessages)]
    public async Task AddReply(cc c, string trigger, string response) {
        if (string.IsNullOrWhiteSpace(trigger) || string.IsNullOrWhiteSpace(response)) {
            await c.RespondAsync($"Incorrect format. Please use the following format: `{BuildInfo.Config.Prefix}AddReply [\"Trigger\"] [\"Response\"]`\n" +
                           "_**Note:** Use \"Quotations\" to separate the trigger from the response._");
            return;
        }
        ReplyStructure.AddValue(trigger, response, false, c.Guild.Id);
        await c.RespondAsync("Saved trigger!");
    }
    
    [Command("AddReply"), Description("Adds an auto response for the server")]
    [RequireOwner]
    public async Task AddReply(cc c, string trigger, string response, string isGlobal) {
        if (string.IsNullOrWhiteSpace(trigger) || string.IsNullOrWhiteSpace(response)) {
            await c.RespondAsync($"Incorrect format. Please use the following format: `{BuildInfo.Config.Prefix}AddReplyGlobal [\"Trigger\"] [\"Response\"] [\"IsGlobal\"]`\n" +
                                 "_**Note:** Use \"Quotations\" to separate the trigger from the response._");
            return;
        }

        var b = isGlobal.ToLower() switch {
            "true" => true,
            "t" => true,
            _ => false
        };
        
        ReplyStructure.AddValue(trigger, response, b);
        await c.RespondAsync($"Saved trigger{(b ? " as a global response" : "")}!");
    }
}