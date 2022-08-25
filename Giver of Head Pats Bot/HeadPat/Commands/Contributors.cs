using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HeadPats.Data;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Contributors : BaseCommandModule {
    public Contributors() => Logger.Loadodule("Contributors");

    [Command("AddContributor"), Aliases("ac"), Description("Adds a Contributor to the list")]
    [RequireOwner]
    public async Task AddContributor(cc c, [Description("Username or alias name to add")] string userName, [RemainingText, Description("Information about what they did")] string info) {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(info)) {
            await c.RespondAsync("Incorrect format. Please use the following format: " +
                                 $"`{BuildInfo.Config.Prefix}AddContributor [\"UserName\"] [\"Info\"]`\n");
            return;
        }
        
        ContributorStructure.AddValue(userName, info);
        await c.RespondAsync("Added and saved Contributor Info!");
    }

    [Command("RemoveContributor"), Aliases("rc"), Description("Removes a Contributor from the list")]
    [RequireOwner]
    public async Task RemoveContributor(cc c, [Description("Username or alias name to remove")] string userName) {
        if (string.IsNullOrWhiteSpace(userName)) {
            await c.RespondAsync("Incorrect format. Please use the following format: " +
                                 $"`{BuildInfo.Config.Prefix}RemoveContributor [\"UserName\"]`\n");
            return;
        }
        
        ContributorStructure.RemoveValue(userName);
        await c.RespondAsync("Removed and saved Contributor Info!");
    }

    [Command("ListContributors"), Aliases("contributors", "lc"), Description("Lists the Contributors of the bot")]
    public async Task ListContributors(cc c) {
        var e = new DiscordEmbedBuilder();
        e.WithColor(DiscordColor.Aquamarine);
        e.WithDescription("These are the Contributors of this bot's project, as I must give credit where its due.");
        foreach (var co in ContributorStructure.Base.Base)
            e.AddField(co.UserName, co.Info.Replace("<br>", "\n"));
        e.WithFooter($"{BuildInfo.Name} (v{BuildInfo.Version}) • {BuildInfo.BuildDate}");
        var bot = await c.Client.GetUserAsync(BuildInfo.ClientId);
        e.WithThumbnail(bot.GetAvatarUrl(ImageFormat.Auto));
        e.WithTitle("Contributors");
        await c.RespondAsync(e.Build());
    }
}