using Discord;
using HeadPats.Commands.Slash.Owner;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;
using Serilog;

namespace HeadPats.Managers.Loops; 

public static class DailyPatLoop {

    public static async Task DoDailyPat(Context db, long currentEpoch) {
        //return;
        if (Vars.IsDebug) return;
        var configGuildSettings = Config.Base.GuildSettings;
        if (configGuildSettings is null) return;
        var updated = false;
        
        foreach (var guild in configGuildSettings) {
            if (guild.GuildId is not 805663181170802719) continue; // for testing purposes only
            try {
                var guildVar = Program.Instance.GetGuild(guild.GuildId);
                if (guildVar is null) {
                    continue;
                }
            }
            catch {/*ignore*/}
            
            if (guild.DailyPats is null || guild.DailyPats.Count is 0) continue;
                
            var guildSettings = Config.GuildSettings(guild.GuildId);
                
            if (guild.DailyPatChannelId is 0)
                continue;
            
            if (guildSettings!.DailyPats is null)
                continue;
                
            foreach (var user in guildSettings.DailyPats) {
                if (user.SetEpochTime >= currentEpoch)
                    continue;
                
                Log.Debug("Trying to daily pat user: {user} ({userId})", user.UserName, user.UserId);
                var guildUser = Program.Instance.Client.GetGuild(guild.GuildId).GetUser(user.UserId);
                if (guildUser is null) {
                    var configDailyPatUser = guildSettings.DailyPats.FirstOrDefault(u => u.UserId.Equals(user.UserId));
                    guild.DailyPats.Remove(configDailyPatUser!);
                    Config.Save();
                    Log.Debug("User not found in guild, skipping and removing from config");
                    continue;
                }
                
                var dbUser = db.Users.AsQueryable().ToList().FirstOrDefault(u => u.UserId.Equals(user.UserId))!;
                var userPatCount = dbUser.PatCount;

                string patUrl;
                if (Vars.UseLocalImages)
                    patUrl = LocalImages.GetRandomPat();
                else {
                    var flux = await Program.Instance.FluxpointClient!.Gifs.GetPatAsync();
                    patUrl = flux.file;
                }
                
                var embed = new EmbedBuilder {
                    Title = "Daily Pats!",
                    Description = $"{user.UserName.ReplaceName(user.UserId)}, You have received your daily pats! You now have {userPatCount + 1} pats!",
                    Color = Colors.Random,
                    ImageUrl = patUrl,
                    Footer = new EmbedFooterBuilder {
                        Text = $"Powered by {(Vars.UseLocalImages ? "CookieAPI" : "Fluxpoint API")}"
                    }
                }.Build();
            
                var channel = Program.Instance.GetChannel(guild.GuildId, guild.DailyPatChannelId);
                
                try {
                    await channel.SendMessageAsync(embed: embed);
                }
                catch {
                    Log.Debug("Failed to send message to channel, skipping");
                    continue;
                }
                
                UserControl.AddPatToUser(user.UserId, 1, false);
                user.SetEpochTime += 86400;
                updated = true;
            }

            if (!updated) continue;
            Config.Save();
        }
    }
}