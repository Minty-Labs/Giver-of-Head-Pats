using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;

namespace HeadPats.Managers.Loops; 

public static class DailyPatLoop {
    public static Dictionary<ulong, bool> DailyPatted;
    // public static Dictionary<ulong, string> PreviousPatUrl;

    public static async Task DoDailyPat(Context db, long currentEpoch) {
        foreach (var guild in Config.Base.GuildSettings!) {
            if (guild.DailyPats is null) continue;
                
            var guildSettings = Config.GuildSettings(guild.GuildId);
            var updated = false;
                
            if (guild.DailyPatChannelId is 0)
                continue;
                
            var channel = await Program.Client!.GetChannelAsync(guild.DailyPatChannelId);
            Users? tempUser = null;
            
            if (guildSettings!.DailyPats is null)
                continue;
                
            foreach (var user in guildSettings.DailyPats) {
                if (user.SetEpochTime > currentEpoch)
                    continue;
                if (channel is null)
                    continue;
                var dbUser = db.Users.AsQueryable().ToList().FirstOrDefault(u => u.UserId.Equals(user.UserId))!;
                if (DailyPatted.ContainsKey(dbUser.UserId) && DailyPatted[dbUser.UserId])
                    continue;
                tempUser = dbUser;
                var userPatCount = dbUser.PatCount;

                string patUrl;
                if  (Vars.UseCookieApi)
                    patUrl = Program.CookieClient!.GetPat();
                else {
                    var flux = await Program.FluxpointClient!.Gifs.GetPatAsync();
                    patUrl = flux.file;
                }
                
                var embed = new DiscordEmbedBuilder {
                    Title = "Daily Pats!",
                    Description = $"{user.UserName.ReplaceName(user.UserId)}, You have received your daily pats! You now have {userPatCount} pats!",
                    Color = DiscordColor.Yellow,
                    ImageUrl = patUrl,
                    Footer = new DiscordEmbedBuilder.EmbedFooter {
                        Text = $"Powered by {(Vars.UseCookieApi ? "CookieAPI" : "Fluxpoint API")}"
                    }
                }.Build();
                
                await channel.SendMessageAsync(embed);
                
                UserControl.AddPatToUser(user.UserId, 1, false);
                user.SetEpochTime += 86400;
                updated = true;
                DailyPatted.TryAdd(dbUser.UserId, updated);
            }

            if (!updated) continue;
            Config.Save();
            DailyPatted[tempUser!.UserId] = false;
        }
    }
}