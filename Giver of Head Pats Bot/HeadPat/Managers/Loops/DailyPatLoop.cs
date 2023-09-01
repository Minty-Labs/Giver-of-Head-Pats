using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;

namespace HeadPats.Managers.Loops; 

public static class DailyPatLoop {
    public static Dictionary<ulong, bool> DailyPatted;
    // public static Dictionary<ulong, string> PreviousPatUrl;

    public static void DoDailyPat(Context db, long currentEpoch) {
        foreach (var guild in Config.Base.GuildSettings!) {
            if (guild.DailyPats is null) continue;
                
            var guildSettings = Config.GuildSettings(guild.GuildId);
            var updated = false;
                
            if (guild.DailyPatChannelId is 0)
                continue;
                
            var channel = Program.Client!.GetChannelAsync(guild.DailyPatChannelId).GetAwaiter().GetResult();
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

                // getPatUrl:
                var patUrl = Vars.UseCookieApi ? Program.CookieClient!.GetPat() : Program.FluxpointClient!.Gifs.GetPatAsync().GetAwaiter().GetResult().file;
                // if (PreviousPatUrl.TryGetValue(guild.GuildId, out patUrl))
                //     goto getPatUrl;
                // PreviousPatUrl.Remove(guild.GuildId);
                // PreviousPatUrl.Add(guild.GuildId, patUrl!);
                
                var embed = new DiscordEmbedBuilder {
                    Title = "Daily Pats!",
                    Description = $"{user.UserName.ReplaceName(user.UserId)}, You have received your daily pats! You now have {userPatCount} pats!",
                    Color = DiscordColor.Yellow,
                    ImageUrl = patUrl,
                    Footer = new DiscordEmbedBuilder.EmbedFooter {
                        Text = $"Powered by {(Vars.UseCookieApi ? "CookieAPI" : "Fluxpoint API")}"
                    }
                }.Build();
                
                channel.SendMessageAsync(embed).GetAwaiter().GetResult();
                
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