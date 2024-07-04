using HeadPats.Configuration;
using HeadPats.Data;
using Quartz;
using Serilog;

namespace HeadPats.Managers.Loops.Jobs;

public class DataDeletionJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        await using var db = new Context();
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        try {
            if (Vars.IsDebug) return;
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
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync("Data Deletion:", obj: err);
        }
    }
}