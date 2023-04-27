using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Data;

namespace HeadPats.Commands.Slash.Contributors; 

public class Contributors : ApplicationCommandModule {
    [SlashCommandGroup("Contributor", "Contributor Commands")]
    public class Contributor : ApplicationCommandModule {
        [SlashCommand("Add", "Adds a Contributor to the list", false), SlashRequireOwner]
        public async Task AddContributor(InteractionContext c, [Option("UserName", "Username or alias name to add", true)] string userName,
            [Option("Info", "Information about what they did", true)] string info) {
            ContributorStructure.AddValue(userName, info);
            await c.CreateResponseAsync("Added and saved Contributor Info!", true);
        }

        [SlashCommand("Remove", "Removes a Contributor from the list", false), SlashRequireOwner]
        public async Task RemoveContributor(InteractionContext c, [Option("UserName", "Username or alias name to remove", true)] string userName) {
            ContributorStructure.RemoveValue(userName);
            await c.CreateResponseAsync("Removed and saved Contributor Info!", true);
        }

        [SlashCommand("List", "Lists the Contributors of the bot")]
        public async Task ListContributors(InteractionContext c) {
            var bot = await c.Client.GetUserAsync(Vars.ClientId);
            var embed = new DiscordEmbedBuilder {
                Title = "Contributors",
                Description = "These are the Contributors of this bot's project, as I must give credit where its due.",
                Color = DiscordColor.Aquamarine,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = bot.GetAvatarUrl(ImageFormat.Auto) },
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"{Vars.Name} (v{Vars.Version}) | {Vars.BuildDate}" }
            };
            foreach (var co in ContributorStructure.Base.Base)
                embed.AddField(co.UserName, co.Info.Replace("<br>", "\n"));
            
            await c.CreateResponseAsync(embed.Build());
        }
    }
}