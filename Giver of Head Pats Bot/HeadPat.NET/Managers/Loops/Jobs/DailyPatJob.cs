using HeadPats.Data;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class DailyPatJob  : IJob {
    private int _numberOfPatErrored;
    public async Task Execute(IJobExecutionContext context) {
        await using var db = new Context();
        var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        try {
            await DailyPatLoop.DoDailyPat(db, currentEpoch);
        }
        catch (Exception err) {
            if (_numberOfPatErrored >= 5) return;
            await DNetToConsole.SendErrorToLoggingChannelAsync($"Daily Pats:\n{err}");
            _numberOfPatErrored++;
        }
    }
}