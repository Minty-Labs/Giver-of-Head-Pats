using HeadPats.Configuration;
using HeadPats.Data;
using Serilog;

namespace HeadPats.Managers.Loops; 

public static class DataDeletionFinderLoop {
    private static readonly ILogger Logger = Log.ForContext(typeof(DataDeletionFinderLoop));
    public static void FindDataDeletion(Context db, long currentEpoch) {
        if (Vars.IsDebug) return;
        var dbGuilds = db.Guilds;
        foreach (var guild in dbGuilds) {
            if (guild.DataDeletionTime == 0) return;
            if (guild.DataDeletionTime >= currentEpoch) continue;
            // Config.Base.GuildSettings!.Remove(guild);
            dbGuilds.Remove(guild);
            var dbGuild = db.Guilds.AsQueryable().ToList().FirstOrDefault(g => g.GuildId.Equals(guild.GuildId));
            if (dbGuild is not null) db.Guilds.Remove(dbGuild);
            Logger.Information("Removed a guild from config and database");
        }
    }
}