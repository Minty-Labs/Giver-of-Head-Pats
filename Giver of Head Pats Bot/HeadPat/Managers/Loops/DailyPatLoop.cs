using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers.Loops; 

public static class DailyPatLoop {
    public static void DoDailyPat(Context db, long currentEpoch) {
        foreach (var guild in Config.Base.GuildSettings!) {
            if (guild.DailyPats is null) continue;
                
            var guildSettings = Config.GuildSettings(guild.GuildId);
            var updated = false;
                
            if (guild.DailyPatChannelId is 0)
                continue;
                
            var channel = Program.Client!.GetChannelAsync(guild.DailyPatChannelId).GetAwaiter().GetResult();
            
            if (guildSettings!.DailyPats is null)
                continue;
                
            foreach (var user in guildSettings.DailyPats) {
                if (user.SetEpochTime > currentEpoch)
                    continue;
                if (channel is null)
                    continue;
                
                var userPatCount = db.Users.AsQueryable().ToList().FirstOrDefault(u => u.UserId.Equals(user.UserId))!.PatCount;
                
                var embed = new DiscordEmbedBuilder {
                    Title = "Daily Pats!",
                    Description = $"{user.UserName.ReplaceName(user.UserId)}, You have received your daily pats! You now have {userPatCount} pats!",
                    Color = DiscordColor.Yellow,
                    ImageUrl = Vars.UseCookieApi ? Program.CookieClient!.GetPat() : Program.FluxpointClient!.Gifs.GetPatAsync().GetAwaiter().GetResult().file,
                    Footer = new DiscordEmbedBuilder.EmbedFooter {
                        Text = $"Powered by {(Vars.UseCookieApi ? "CookieAPI" : "Fluxpoint API")}"
                    }
                }.Build();
                
                channel.SendMessageAsync(embed).GetAwaiter().GetResult();
                
                Data.Models.UserControl.AddPatToUser(user.UserId, 1, false);
                user.SetEpochTime += 86400;
                updated = true;
            }

            if (updated) {
                Config.Save();
            }
        }
    }
}