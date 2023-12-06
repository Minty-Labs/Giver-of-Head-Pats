using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Configuration;
using HeadPats.Configuration.Classes;
using HeadPats.Managers;

namespace HeadPats.Commands.Slash; 

public class DailyPatCmds : InteractionModuleBase<SocketInteractionContext> {

    [Group("dailypats", "Daily Pats commands"), EnabledInDm(false), RequireUserPermission(GuildPermission.Administrator)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        
        [SlashCommand("setchannel", "Sets the channel where daily pats are sent")]
        public async Task SetDailyPatChannel([Summary("channel", "Channel to set as the daily pat channel")] ITextChannel channel) {
            var guildSettings = Config.GuildSettings(Context.Guild.Id);
            guildSettings!.DailyPatChannelId = channel.Id;
            Config.Save();
            await RespondAsync($"Set the daily pat channel to <#{channel.Id}>");
        }

        private static bool _doesItExist(ISnowflakeEntity user, ulong guildId) => Config.GuildSettings(guildId)!.DailyPats!.Any(u => u.UserId == user.Id); 
        
        [SlashCommand("add", "Sets the daily pat to user")]
        public async Task AddDailyPat([Summary("user", "Sets the daily pat to user")] IUser user) {
            // is user in guild
            if (user is not IGuildUser) {
                await RespondAsync("User must be in the guild.", ephemeral: true);
                return;
            }
            
            // is not role
            // if (user is IRole) {
            //     await RespondAsync("You cannot set a daily pat to a role; it must be a DiscordMember");
            //     return;
            // }
            
            // does it exist
            if (_doesItExist(user, Context.Guild.Id)) {
                await RespondAsync("User already has a daily pat set.", ephemeral: true);
                return;
            }
            
            var dailyPat = new DailyPat {
                UserId = user.Id,
                UserName = user.Username,
                SetEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 86400
            };
            var guildSettings = Config.GuildSettings(Context.Guild.Id);
        
            guildSettings!.DailyPats!.Add(dailyPat);
            Config.Save();
            await RespondAsync($"Set daily pat for {user.Username.ReplaceName(user.Id)}.");
        }

        [SlashCommand("remove", "Removes the daily pat from user")]
        public async Task RemoveDailyPat([Summary("user", "Removes the daily pat from user")] IUser user) {
            if (!_doesItExist(user, Context.Guild.Id)) {
                await RespondAsync("User does not have a daily pat set.", ephemeral: true);
                return;
            }

            var guildSettings = Config.GuildSettings(Context.Guild.Id);

            var dailyPat = guildSettings!.DailyPats!.Single(u => u.UserId == user.Id);
            guildSettings!.DailyPats!.Remove(dailyPat);
            Config.Save();
            await RespondAsync($"Removed daily pat from {user.Username.ReplaceName(user.Id)}.");
        }

        [SlashCommand("list", "Lists all users with daily pats set")]
        public async Task ListDailyPats() {
            var sb = new StringBuilder();
            sb.AppendLine("`UserName (ID) - Next Pat Time`");
            var guildSettings = Config.GuildSettings(Context.Guild.Id);
            foreach (var dailyPat in guildSettings!.DailyPats!) {
                sb.AppendLine($"{dailyPat.UserName.ReplaceName(dailyPat.UserId)} ({dailyPat.UserId}) - <t:{dailyPat.SetEpochTime}:>");
            }

            await RespondAsync(sb.ToString());
        }

    }
    
}