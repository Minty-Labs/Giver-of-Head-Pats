using Discord;
using Discord.WebSocket;
using HeadPats.Configuration;
using HeadPats.Data;
using HeadPats.Data.Models;
using HeadPats.Utils;
using HeadPats.Utils.ExternalApis;
using Serilog;

namespace HeadPats.Managers.Loops;

public static class DailyPatLoop {
    private static readonly ILogger Logger = Log.ForContext(typeof(DailyPatLoop));
    
    public static async Task DoDailyPat(Context db, long currentEpoch) {
        var dp = DailyPatConfig.Base;
        if (dp is null) return;
        var updated = false;

        foreach (var guild in dp.Guilds!) {
            SocketGuild? socketGuild;
            try {
                socketGuild = Program.Instance.Client.GetGuild(guild.GuildId);
                if (socketGuild is null) {
                    continue;
                }
            }
            catch {
                // ignore
                continue;
            }
            
            if (guild.Users is null || guild.Users.Count is 0) continue;
            
            var guildConfig = dp.Guilds.FirstOrDefault(g => g.GuildId == guild.GuildId);
            if (guildConfig is null || guild.DailyPatChannelId is 0 || guildConfig.Users is null) continue;

            foreach (var user in guildConfig.Users.ToList()) {
                if (user.SetEpochTime >= currentEpoch)
                    continue;
                
                var guildUser = socketGuild.GetUser(user.UserId);
                Logger.Information("Trying to daily pat user: {user} ({userId})", guildUser.Username, user.UserId);
                var listOfUsersInGuild = socketGuild.Users;
                var userInGuild = listOfUsersInGuild.FirstOrDefault(u => u.Id == user.UserId);
                if (userInGuild is null) {
                    var configDailyPatUser = guildConfig.Users.FirstOrDefault(u => u.UserId.Equals(user.UserId));
                    guildConfig.Users.Remove(configDailyPatUser!);
                    DailyPatConfig.Save();
                    Logger.Information("User not found in guild, skipping and removing from config");
                    continue;
                }
                
                var dbUser = db.Users.AsQueryable().ToList().FirstOrDefault(u => u.UserId.Equals(user.UserId))!;
                var userPatCount = dbUser.PatCount;
                
                string patUrl;
                if (Vars.UseLocalImages)
                    patUrl = LocalImages.GetRandomImage(Category.Pat);
                else {
                    var flux = await Program.Instance.FluxpointClient!.Gifs.GetPatAsync();
                    patUrl = flux.file;
                }
                
                var embed = new EmbedBuilder {
                    Title = "Daily Pats!",
                    Description = $"{userInGuild.Username.ReplaceName(user.UserId)}, You have received your daily pats! You now have {userPatCount + 1} pats!",
                    Color = Colors.Random,
                    ImageUrl = patUrl,
                    Footer = new EmbedFooterBuilder {
                        Text = $"Powered by {(Vars.UseLocalImages ? "the community" : "Fluxpoint API")}"
                    }
                }.Build();
                
                SocketTextChannel? channel = null;

                try {
                    channel = await Program.Instance.Client.GetChannelAsync(guild.DailyPatChannelId) as SocketTextChannel;
                }
                catch {
                    channel = (SocketTextChannel)socketGuild.GetChannel(guild.DailyPatChannelId);
                }
                finally {
                    if (channel is not null)
                        await channel.SendMessageAsync(embed: embed);
                    else
                        Logger.Information("[BAD] Channel not found :: g:{guildID} c:{channelID}", guild.GuildId, guild.DailyPatChannelId);
                }

                UserControl.AddPatToUser(user.UserId, 1);
                user.SetEpochTime += 86400;
                updated = true;
            }
        }
        
        if (!updated) return;
        DailyPatConfig.Save();
    }
}