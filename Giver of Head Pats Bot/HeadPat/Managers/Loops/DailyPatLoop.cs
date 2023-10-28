using DSharpPlus.Entities;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using Serilog;

namespace HeadPats.Managers.Loops; 

public static class DailyPatLoop {
    public static Dictionary<ulong, bool> DailyPatted = new ();
    public static List<ulong> FailedPatChannels = new ();
    // public static Dictionary<ulong, string> PreviousPatUrl;

    public static async Task DoDailyPat(Context db, long currentEpoch) {
        if (Vars.IsDebug) return;
        var configGuildSettings = Config.Base.GuildSettings;
        if (configGuildSettings is null) return;
        
        foreach (var guild in configGuildSettings) {
            if (guild.DailyPats is null) continue;
                
            var guildSettings = Config.GuildSettings(guild.GuildId);
            var updated = false;
                
            if (guild.DailyPatChannelId is 0)
                continue;
            
            var discordGuild = await Program.Client!.GetGuildAsync(guild.GuildId);
                
            // var channel = await Program.Client!.GetChannelAsync(guild.DailyPatChannelId);
            discordGuild.Channels.TryGetValue(guild.DailyPatChannelId, out var channel);
            if (channel is null) {
                FailedPatChannels.Add(guild.DailyPatChannelId);
                Log.Debug("Target pat channel {chanId} not found, skipping", guild.DailyPatChannelId);
                continue;
            }
            
            Users? tempUser = null;
            
            if (guildSettings!.DailyPats is null)
                continue;
                
            foreach (var user in guildSettings.DailyPats) {
                if (user.SetEpochTime >= currentEpoch)
                    continue;
                
                Log.Debug("Trying to daily pat user: {user} ({userId})", user.UserName, user.UserId);
                var guildUser = await discordGuild.GetMemberAsync(user.UserId);
                // discordGuild.Members.TryGetValue(user.UserId, out var guildUser);
                if (guildUser is null) {
                    Log.Debug("User not found in guild, skipping");
                    continue;
                }
                
                var dbUser = db.Users.AsQueryable().ToList().FirstOrDefault(u => u.UserId.Equals(user.UserId))!;
                if (DailyPatted.TryGetValue(dbUser.UserId, out var value) && value)
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