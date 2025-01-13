using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Managers;

namespace HeadPats.Commands.Slash; 

public class DailyPatCmds : InteractionModuleBase<SocketInteractionContext> {

    [Group("dailypats", "Daily Pats commands"), RequireUserPermission(GuildPermission.Administrator), CommandContextType(InteractionContextType.Guild)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        
        [SlashCommand("setchannel", "Sets the channel where daily pats are sent")]
        public async Task SetDailyPatChannel([Summary("channel", "Channel to set as the daily pat channel")] ITextChannel channel) {
            if (Context.Guild.Id is not 805663181170802719) {
                await RespondAsync("This command is only available in the testing server for the time being.", ephemeral: true);
                return;
            }
            var guildConfig = DailyPatConfig.Base.Guilds!.FirstOrDefault(g => g.GuildId == Context.Guild.Id);
            guildConfig!.DailyPatChannelId = channel.Id;
            Config.Save();
            await RespondAsync($"Set the daily pat channel to <#{channel.Id}>");
        }

        private static bool _doesItExist(ISnowflakeEntity user, ulong guildId) => DailyPatConfig.Base.Guilds!.FirstOrDefault(g => g.GuildId == guildId)!.Users!.Any(u => u.UserId == user.Id);
        
        [SlashCommand("add", "Sets the daily pat to user")]
        public async Task AddDailyPat([Summary("user", "Sets the daily pat to user")] IUser user) {
            if (Context.Guild.Id is not 805663181170802719) {
                await RespondAsync("This command is only available in the testing server for the time being.", ephemeral: true);
                return;
            }
            // is user in guild
            if (user is not IGuildUser) {
                await RespondAsync("User must be in the guild.", ephemeral: true);
                return;
            }
            
            if (_doesItExist(user, Context.Guild.Id)) {
                await RespondAsync("User already has a daily pat set.", ephemeral: true);
                return;
            }
            
            var dailyPat = new DailyPatUser {
                UserId = user.Id,
                SetEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 86400
            };
            
            var guildConfig = DailyPatConfig.Base.Guilds!.FirstOrDefault(g => g.GuildId == Context.Guild.Id);
        
            guildConfig!.Users!.Add(dailyPat);
            Config.Save();
            await RespondAsync($"Set daily pat for {user.Username.ReplaceName(user.Id)}.");
        }

        [SlashCommand("remove", "Removes the daily pat from user")]
        public async Task RemoveDailyPat([Summary("user", "Removes the daily pat from user")] IUser user) {
            if (Context.Guild.Id is not 805663181170802719) {
                await RespondAsync("This command is only available in the testing server for the time being.", ephemeral: true);
                return;
            }
            if (!_doesItExist(user, Context.Guild.Id)) {
                await RespondAsync("User does not have a daily pat set.", ephemeral: true);
                return;
            }

            var guildConfig = DailyPatConfig.Base.Guilds!.FirstOrDefault(g => g.GuildId == Context.Guild.Id);

            var dailyPat = guildConfig!.Users!.Single(u => u.UserId == user.Id);
            guildConfig!.Users!.Remove(dailyPat);
            Config.Save();
            await RespondAsync($"Removed daily pat from {user.Username.ReplaceName(user.Id)}.");
        }

        [SlashCommand("list", "Lists all users with daily pats set")]
        public async Task ListDailyPats() {
            var sb = new StringBuilder();
            sb.AppendLine("Daily Pats are currently not working. Will be fixed soon.");
            sb.AppendLine("`UserName (ID) - Next Pat Time`");
            
            var guildConfig = DailyPatConfig.Base.Guilds!.FirstOrDefault(g => g.GuildId == Context.Guild.Id);
            
            foreach (var dailyPat in guildConfig!.Users!) {
                var guildUser = Context.Guild.GetUser(dailyPat.UserId);
                sb.AppendLine($"{guildUser.Username.ReplaceName(dailyPat.UserId)} ({dailyPat.UserId}) - <t:{dailyPat.SetEpochTime}:>");
            }

            await RespondAsync(sb.ToString());
        }

    }
    
}