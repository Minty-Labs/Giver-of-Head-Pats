using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using HeadPats.Configuration;
using HeadPats.Managers;
using Serilog;

namespace HeadPats.Commands.Slash.Contributors; 

public class Contributors : ApplicationCommandModule {
    [SlashCommandGroup("Contributor", "Contributor Commands"), Hidden]
    public class Contributor : ApplicationCommandModule {
        [SlashCommand("Add", "Adds a Contributor to the list", false), SlashRequireOwner]
        public async Task AddContributor(InteractionContext c, [Option("UserName", "Username or alias name to add", true)] string userName,
            [Option("Info", "Information about what they did (you can use <br>)", true)] string info) {
            var doesUserNameExist = Config.Base.Contributors!.FirstOrDefault(n => n.UserName == userName)?.UserName == userName;
            
            if (doesUserNameExist) {
                Log.Debug("Removing duplicate user");
                var itemToRemove = Config.Base.Contributors!.Single(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
                Config.Base.Contributors!.Remove(itemToRemove);
            }

            var item = new Configuration.Contributor {
                UserName = userName,
                Info = info
            };
            Config.Base.Contributors!.Add(item);
            Config.Save();
            await c.CreateResponseAsync("Added and saved Contributor Info!", true);
        }

        [SlashCommand("Remove", "Removes a Contributor from the list", false), SlashRequireOwner]
        public async Task RemoveContributor(InteractionContext c, [Option("UserName", "Username or alias name to remove", true)] string userName) {
            var doesUserNameExist = Config.Base.Contributors!.FirstOrDefault(n => n.UserName == userName)?.UserName == userName;
            
            if (!doesUserNameExist) return;
            try {
                var contributor = Config.Base.Contributors!.Single(u => u.UserName == userName);
                Config.Base.Contributors!.Remove(contributor);
            }
            catch (Exception e) {
                await DSharpToConsole.SendErrorToLoggingChannelAsync(e);
            }
            Config.Save();

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
            foreach (var co in Config.Base.Contributors!)
                embed.AddField(co.UserName, co.Info!.Replace("<br>", "\n"));
            
            await c.CreateResponseAsync(embed.Build());
        }
    }
}