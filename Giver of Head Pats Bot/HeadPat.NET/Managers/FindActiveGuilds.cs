using HeadPats.Configuration;
using HeadPats.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HeadPats.Managers;

public class FindActiveGuilds {
    private static readonly ILogger Logger = Log.ForContext(typeof(FindActiveGuilds));
    public static async void Start() {
        if (Vars.IsDebug) return;
        await using var db = new Context();
        var dbGuild = db.Guilds.AsQueryable();
        var needsUpdating = false;
        foreach (var guild in dbGuild) {
            try {
                var guildVar = Program.Instance.GetGuild(guild.GuildId);
                if (guildVar is not null) continue;
                var guildFromConfig = dbGuild.Single(x => x.GuildId == guild.GuildId);
                guildFromConfig.DailyPatChannelId = 0;
                await db.DailyPats.Where(x => x.GuildId == guild.GuildId).ExecuteDeleteAsync();
                if (guildFromConfig.DataDeletionTime == 0)
                    guildFromConfig.DataDeletionTime = DateTimeOffset.UtcNow.AddDays(28).ToUnixTimeSeconds();
                needsUpdating = true;
                Logger.Debug("Guild {guildId} not found, skipping and removing guild from config", guild.GuildId);
            }
            catch { /*ignore*/}
        }

        if (needsUpdating)
            await db.SaveChangesAsync();
    }
}