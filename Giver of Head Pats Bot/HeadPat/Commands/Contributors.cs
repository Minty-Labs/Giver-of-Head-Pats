using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;
using cc = DSharpPlus.CommandsNext.CommandContext;
using ic = DSharpPlus.SlashCommands.InteractionContext;

namespace HeadPats.Commands; 

public class Contributors : ApplicationCommandModule {
    [SlashCommandGroup("Contributor", "Contributor Commands")]
    public class Contributor : ApplicationCommandModule {
        [SlashCommand("Add", "Adds a Contributor to the list", false)]
        [SlashRequireOwner]
        public async Task AddContributor(ic c, [Option("UserName", "Username or alias name to add", true)] string userName,
            [Option("Info", "Information about what they did", true)] string info) {
            ContributorStructure.AddValue(userName, info);
            await c.CreateResponseAsync("Added and saved Contributor Info!", true);
        }

        [SlashCommand("Remove", "Removes a Contributor from the list", false)]
        [SlashRequireOwner]
        public async Task RemoveContributor(ic c, [Option("UserName", "Username or alias name to remove", true)] string userName) {
            ContributorStructure.RemoveValue(userName);
            await c.CreateResponseAsync("Removed and saved Contributor Info!", true);
        }

        [SlashCommand("List", "Lists the Contributors of the bot")]
        public async Task ListContributors(ic c) {
            var e = new DiscordEmbedBuilder();
            e.WithColor(DiscordColor.Aquamarine);
            e.WithDescription("These are the Contributors of this bot's project, as I must give credit where its due.");
            foreach (var co in ContributorStructure.Base.Base)
                e.AddField(co.UserName, co.Info.Replace("<br>", "\n"));
            e.WithFooter($"{Vars.Name} (v{Vars.Version}) • {Vars.BuildDate}");
            var bot = await c.Client.GetUserAsync(Vars.ClientId);
            e.WithThumbnail(bot.GetAvatarUrl(ImageFormat.Auto));
            e.WithTitle("Contributors");
            await c.CreateResponseAsync(e.Build());
        }
    }
}