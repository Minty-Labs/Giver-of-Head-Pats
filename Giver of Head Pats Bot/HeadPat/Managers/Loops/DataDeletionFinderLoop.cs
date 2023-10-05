using HeadPats.Configuration;
using HeadPats.Data;
using Serilog;

namespace HeadPats.Managers.Loops; 

public static class DataDeletionFinderLoop {
    public static void FindDataDeletion(Context db, long currentEpoch) {
        var configGuildSettings = Config.Base.GuildSettings!;
        foreach (var guild in configGuildSettings) {
            if (guild.DataDeletionTime == 0) return;
            if (guild.DataDeletionTime >= currentEpoch) continue;
            Config.Base.GuildSettings!.Remove(guild);
            var dbGuild = db.Guilds.AsQueryable().ToList().FirstOrDefault(g => g.GuildId.Equals(guild.GuildId));
            if (dbGuild is not null) db.Guilds.Remove(dbGuild);
            Log.Information("[DataDeletionFinderLoop] Removed a guild from config and database");
        }
    }
}