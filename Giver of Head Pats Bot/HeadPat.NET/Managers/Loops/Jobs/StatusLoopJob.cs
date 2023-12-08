using HeadPats.Data;
using Quartz;

namespace HeadPats.Managers.Loops.Jobs;

public class StatusLoopJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        await using var db = new Context();
        try {
            await StatusLoop.Update(db);
        }
        catch (Exception err) {
            await DNetToConsole.SendErrorToLoggingChannelAsync($"Status:\n{err}");
        }
    }
}