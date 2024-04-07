using Discord;
using HeadPats.Configuration;
using HeadPats.Data;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class StatusLoopJob : IJob {
    private static int _tempPatCount;
    
    public async Task Execute(IJobExecutionContext context) {
        await using var db = new Context();
        try {
            if (Config.Base.RotatingStatus.Enabled || Vars.IsDebug) return;
            var tempPatCount = db.Overall.AsQueryable().ToList().First().PatCount;

            if (tempPatCount == _tempPatCount) return;
        
            await Program.Instance.Client.SetStatusAsync(UserStatus.Online);
            await Program.Instance.Client.SetActivityAsync(new CustomStatusGame($"{tempPatCount:N0} head pats given"));
            // Log.Debug("Updated Status");
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync($"Status:\n{err}");
        }
    }
}