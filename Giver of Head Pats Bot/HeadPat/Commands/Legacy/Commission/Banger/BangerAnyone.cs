using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HeadPats.Handlers;
using HeadPats.Handlers.Events;

namespace HeadPats.Commands.Legacy.Commission.Banger; 

public class BangerAnyone : BaseCommandModule {
    
    [Command("ListBangerValues"), Description("Lists the current values for the banger system"), InPennysServerAnyone]
    public async Task ListBangerValues(CommandContext ctx) {
        var sb = new StringBuilder();
        sb.AppendLine("```");
        sb.AppendLine("Whitelisted URLs:");
        foreach (var url in BangerEventListener.WhitelistedUrls!) {
            sb.AppendLine($"- {url}");
        }
        sb.AppendLine();
        sb.AppendLine("Whitelisted File Extensions:");
        foreach (var ext in BangerEventListener.WhitelistedFileExtensions!) {
            sb.AppendLine($"- {ext}");
        }
        sb.AppendLine("```");
        await ctx.RespondAsync(sb.ToString());
    }
}