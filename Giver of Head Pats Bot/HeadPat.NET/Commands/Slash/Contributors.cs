using Discord;
using Discord.Interactions;
using HeadPats.Commands.Preexecution;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Managers;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Commands.Slash; 

public class Contributors : InteractionModuleBase<SocketInteractionContext> {
    
    [Group("contributors", "Contributors to this bot's development"), RequireUser(167335587488071682)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        
        [SlashCommand("add", "Add a contributor to the list")]
        public async Task AddContributor([Summary("username", "Username or alias name to add")] string userName,
            [Summary("info", "Information about what they did (you can use <br>)")] string info) {
            var doesUserNameExist = Config.Base.Contributors!.FirstOrDefault(n => n.UserName == userName)?.UserName == userName;
            
            if (doesUserNameExist) {
                Log.Debug("Removing duplicate user");
                var itemToRemove = Config.Base.Contributors!.Single(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
                Config.Base.Contributors!.Remove(itemToRemove);
            }

            var item = new BotContributor {
                UserName = userName,
                Info = info
            };
            Config.Base.Contributors!.Add(item);
            Config.Save();
            await RespondAsync("Added and saved Contributor Info!", ephemeral: true);
        }
        
        [SlashCommand("remove", "Remove a contributor from the list")]
        public async Task RemoveContributor([Summary("username", "Username or alias name to remove")] string userName) {
            var doesUserNameExist = Config.Base.Contributors!.FirstOrDefault(n => n.UserName == userName)?.UserName == userName;
            if (!doesUserNameExist) return;
            try {
                var contributor = Config.Base.Contributors!.Single(u => u.UserName == userName);
                Config.Base.Contributors!.Remove(contributor);
            }
            catch (Exception e) {
                await DNetToConsole.SendErrorToLoggingChannelAsync(e);
            }
            Config.Save();

            await RespondAsync("Removed and saved Contributor Info!", ephemeral: true);
        }
        
        [SlashCommand("list", "Lists the Contributors of the bot")]
        public async Task ListContributors() {
            var bot = Program.Instance.GetUser(Vars.ClientId);
            var embed = new EmbedBuilder {
                Title = "Contributors",
                Description = "These are the Contributors of this bot's project, as I must give credit where its due.",
                Color = Colors.HexToColor("00FFBF"),
                ThumbnailUrl = bot!.GetAvatarUrl(),
                Footer = new EmbedFooterBuilder { Text = "If you would like to be added to this list, please contact me." }
            };
            foreach (var contributor in Config.Base.Contributors!) {
                embed.AddField(contributor.UserName, contributor.Info);
            }
            await RespondAsync(embed: embed.Build());
        }
    }
}