using System.Text;
using Discord;
using Discord.Interactions;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Managers;

namespace HeadPats.Commands.Slash; 

public class DailyPatCmds : InteractionModuleBase<SocketInteractionContext> {

    [Group("dailypats", "Daily Pats commands"), EnabledInDm(false), RequireUserPermission(GuildPermission.Administrator)]
    public class Commands : InteractionModuleBase<SocketInteractionContext> {
        
        [SlashCommand("setchannel", "Sets the channel where daily pats are sent")]
        public async Task SetDailyPatChannel([Summary("channel", "Channel to set as the daily pat channel")] ITextChannel channel) {
            await using var db = new Context();
            var dbGuild = db.Guilds.AsQueryable().FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            dbGuild!.DailyPatChannelId = channel.Id;
            Config.Save();
            await RespondAsync($"Set the daily pat channel to <#{channel.Id}>");
        }

        private static bool _doesItExist(ISnowflakeEntity user, Context db, ulong guildId) => db.DailyPats.AsQueryable().Where(g => g.GuildId == guildId).Any(u => u.UserId == user.Id);
        
        [SlashCommand("add", "Sets the daily pat to user")]
        public async Task AddDailyPat([Summary("user", "Sets the daily pat to user")] IUser user) {
            // is user in guild
            if (user is not IGuildUser) {
                await RespondAsync("User must be in the guild.", ephemeral: true);
                return;
            }
            
            await using var db = new Context();
            
            // does it exist
            if (_doesItExist(user, db, Context.Guild.Id)) {
                await RespondAsync("User already has a daily pat set.", ephemeral: true);
                return;
            }
            
            var dailyPat = new DailyPats {
                UserId = user.Id,
                // UserName = user.Username,
                GuildId = Context.Guild.Id,
                SetEpochTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 86400
            };
        
            db.DailyPats.Add(dailyPat);
            await db.SaveChangesAsync();
            await RespondAsync($"Set daily pat for {user.Username.ReplaceName(user.Id)}.");
        }

        [SlashCommand("remove", "Removes the daily pat from user")]
        public async Task RemoveDailyPat([Summary("user", "Removes the daily pat from user")] IUser user) {
            await using var db = new Context();
            if (!_doesItExist(user, db, Context.Guild.Id)) {
                await RespondAsync("User does not have a daily pat set.", ephemeral: true);
                return;
            }

            var dailyPat = db.DailyPats.AsQueryable().FirstOrDefault(x => x.UserId == user.Id && x.GuildId == Context.Guild.Id);
            db.DailyPats.Remove(dailyPat!);
            await db.SaveChangesAsync();
            await RespondAsync($"Removed daily pat from {user.Username.ReplaceName(user.Id)}.");
        }

        [SlashCommand("list", "Lists all users with daily pats set")]
        public async Task ListDailyPats() {
            await using var db = new Context();
            var sb = new StringBuilder();
            sb.AppendLine("`UserName (ID) - Next Pat Time`");
            
            var guildPats = db.DailyPats.AsQueryable().Where(x => x.GuildId == Context.Guild.Id).ToList();
            if (guildPats.Count is 0) {
                await RespondAsync("No daily pats are set.");
                return;
            }
            
            foreach (var user in guildPats) {
                sb.AppendLine($"{Program.Instance.GetGuildUser(Context.Guild.Id, user.UserId)!.Username.ReplaceName(user.UserId) ?? "\"could not load username\""} ({user.UserId}) - <t:{user.SetEpochTime}:>");
            }

            await RespondAsync(sb.ToString());
        }

    }
    
}